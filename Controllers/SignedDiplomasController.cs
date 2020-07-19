using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eLearning.Data;
using eLearning.Models;
using Omnibasis.BigchainCSharp.Model;
using Omnibasis.BigchainCSharp.Builders;
using NSec.Cryptography;
using Omnibasis.BigchainCSharp.Util;
using Omnibasis.BigchainCSharp.Constants;
using Omnibasis.BigchainCSharp.Api;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace eLearning.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SignedDiplomasController : Controller
    {
        private static String publicKeyString = "302a300506032b657003210033c43dc2180936a2a9138a05f06c892d2fb1cfda4562cbc35373bf13cd8ed373";
        private static String privateKeyString = "302e020100300506032b6570042204206f6b0cd095f1e83fc5f08bffb79c7c8a30e77a3ab65f4bc659026b76394fcea8";

        private readonly ApplicationDbContext _context;

        public SignedDiplomasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SignedDiplomas
        public async Task<IActionResult> Index()
        {
            return View(await _context.SignedDiplomas.ToListAsync());
        }

        // GET: SignedDiplomas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signedDiploma = await _context.SignedDiplomas
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (signedDiploma == null)
            {
                return NotFound();
            }

            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("learning.ici.ro/SignedDiplomas/Validate?transactionId="+signedDiploma.TransactionId, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            var vm = new QrViewModel
            {
                byteArray = BitmapToBytesCode(qrCodeImage),
                signedDiploma = signedDiploma
            };

            return View(vm);
        }

        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        // GET: SignedDiplomas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SignedDiplomas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HolderFullName,DateOfIssue,TransactionId,CourseName")] SignedDiploma signedDiploma)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Sign the transaction to the blockchain
                    var conn1Config = new Dictionary<string, object>();

                    //config connection 1
                    conn1Config.Add("baseUrl", "http://ebcl1.ici.ro");
                    BlockchainConnection conn1 = new BlockchainConnection(conn1Config);

                    var conn2Config = new Dictionary<string, object>();
                    var headers2 = new Dictionary<string, string>();
                    //config connection 2
                    conn2Config.Add("baseUrl", "http://ebcl2.ici.ro");
                    BlockchainConnection conn2 = new BlockchainConnection(conn2Config);

                    var conn3Config = new Dictionary<string, object>();
                    var headers3 = new Dictionary<string, string>();
                    //config connection 2
                    conn3Config.Add("baseUrl", "http://ebcl3.ici.ro");
                    BlockchainConnection conn3 = new BlockchainConnection(conn2Config);

                    //add connections
                    IList<BlockchainConnection> connections = new List<BlockchainConnection>();
                    connections.Add(conn1);
                    connections.Add(conn2);
                    connections.Add(conn3);

                    //multiple connections
                    var builderWithConnections = BigchainDbConfigBuilder
                        .addConnections(connections)
                        .setTimeout(60000); //override default timeout of 20000 milliseconds

                    await builderWithConnections.setup();

                    // prepare your key

                    var algorithm = SignatureAlgorithm.Ed25519;
                    var privateKey = Key.Import(algorithm, Utils.StringToByteArray(privateKeyString), KeyBlobFormat.PkixPrivateKey);
                    var publicKey = PublicKey.Import(algorithm, Utils.StringToByteArray(publicKeyString), KeyBlobFormat.PkixPublicKey);

                    BlockchainTransaction assetData = new BlockchainTransaction();
                    assetData.HoldersName = signedDiploma.HolderFullName;
                    assetData.DateOfIssue = signedDiploma.DateOfIssue;
                    assetData.CourseName = signedDiploma.CourseName;

                    //TestMetadata metaData = new TestMetadata();
                    TransactionsMetadata metaData = new TransactionsMetadata();
                    string externalip = new WebClient().DownloadString("http://icanhazip.com");
                    metaData.SignerIP = externalip;

                    // Set up, sign, and send your transaction
                    var transaction = BigchainDbTransactionBuilder<BlockchainTransaction, TransactionsMetadata>
                        .init()
                            .addAssets(assetData)
                        .addMetaData(metaData)
                        .operation(Operations.CREATE)
                        .buildAndSignOnly(publicKey, privateKey);

                    var createTransaction = await TransactionsApi<BlockchainTransaction, TransactionsMetadata>.sendTransactionAsync(transaction);

                    string assetId2 = "";
                    // the asset's ID is equal to the ID of the transaction that created it
                    if (createTransaction != null && createTransaction.Data != null)
                    {
                        assetId2 = createTransaction.Data.Id;
                        signedDiploma.TransactionId = assetId2;
                        _context.SignedDiplomas.Add(signedDiploma);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else if (createTransaction != null)
                    {
                        return Content("Could not send it: " + createTransaction.Messsage.Message);
                    }
                }
                catch(Exception e)
                {
                    return Content(e.ToString());
                }
                
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        private bool SignedDiplomaExists(int id)
        {
            return _context.SignedDiplomas.Any(e => e.Id == id);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ValidateAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return NotFound();
            }

            try
            {
                var conn1Config = new Dictionary<string, object>();

                //config connection 1
                conn1Config.Add("baseUrl", "http://ebcl1.ici.ro");
                BlockchainConnection conn1 = new BlockchainConnection(conn1Config);

                var conn2Config = new Dictionary<string, object>();
                var headers2 = new Dictionary<string, string>();
                //config connection 2
                conn2Config.Add("baseUrl", "http://ebcl2.ici.ro");
                BlockchainConnection conn2 = new BlockchainConnection(conn2Config);

                var conn3Config = new Dictionary<string, object>();
                var headers3 = new Dictionary<string, string>();
                //config connection 2
                conn3Config.Add("baseUrl", "http://ebcl3.ici.ro");
                BlockchainConnection conn3 = new BlockchainConnection(conn2Config);

                //add connections
                IList<BlockchainConnection> connections = new List<BlockchainConnection>();
                connections.Add(conn1);
                connections.Add(conn2);
                connections.Add(conn3);

                //multiple connections
                var builderWithConnections = BigchainDbConfigBuilder
                    .addConnections(connections)
                    .setTimeout(60000); //override default timeout of 20000 milliseconds

                await builderWithConnections.setup();

                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(transactionId);
                BlockchainTransaction bt = JsonConvert.DeserializeObject<BlockchainTransaction>(testTran2.Asset.Data.ToString());

                return View(bt);

            }
            catch(Exception e)
            {
                return Content("Error connecting to the blockchain");
            }
        }
    }
}
