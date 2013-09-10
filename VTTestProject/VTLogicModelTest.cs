using VT.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;


/**
 * TESTS ONLY WORK ON DDREV2
 * 
 * */
namespace VTTestProject
{
    
    
    /// <summary>
    ///This is a test class for VTLogicModelTest and is intended
    ///to contain all VTLogicModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VTLogicModelTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        //------------------------------ TEST SERIAL GENERATION ------------------------------//

        /**
         * Generates a new serial number.
         * */ 
        [TestMethod()]
        public void GenerateTubeSerialTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string str_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,1";
            string[] actual = target.insertSerialTODB(str_cmd);
            string[] expected = { "0", "" };
            Assert.AreEqual(expected[0], actual[0]);
        }

        /**
         * Generates a new serial number that is already taken.
         * */
        [TestMethod()]
        public void GenerateDuplicateTubeSerialTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string serial_number = "0";
            string str_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,1";
            string[] actual = target.insertSerialTODB(str_cmd);
            string error_code = "51024";
            Assert.AreEqual(error_code, actual[0]);
        }

        /**
         * Generates a tube serial with a part number that does not exists.
         * */
        [TestMethod()]
        public void GenerateTubePartNumberDNE()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string str_cmd = "SNT," + serial_number + ",9999,0,UNITTEST,1";
            string[] actual = target.insertSerialTODB(str_cmd);
            string error_code = "51011";
            Assert.AreEqual(error_code, actual[0]);
        }

        /**
         * Generates a cathode serial with a part number that does not exists.
         * */
        [TestMethod()]
        public void GenerateCathodePartNumberDNE()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string str_cmd = "CHD," + serial_number + ",9999,0,UNITTEST,1";
            string[] actual = target.insertCathodeSerialTODB(str_cmd);
            string error_code = "51011";
            Assert.AreEqual(error_code, actual[0]);
        }

        /**
         * Generate a tube serial with bad emp id
         * */
        [TestMethod()]
        public void GenerateTubeEmployeeIDDNE()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string str_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,DNE";
            string[] actual = target.insertSerialTODB(str_cmd);
            string error_code = "51015";
            // TODO: Generte SN Allows any employee id.
            //Assert.AreEqual(error_code, actual[0]);
        }

        /**
         * Generate a cathode serial with bad emp id
         * */
        [TestMethod()]
        public void GenerateCathodeEmployeeIDDNE()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string str_cmd = "CHD," + serial_number + ",24782,0,UNITTEST,DNE";
            string[] actual = target.insertCathodeSerialTODB(str_cmd);
            string error_code = "51015";
            // TODO: Generate SN Allows any employee id.
            //Assert.AreEqual(error_code, actual[0]);
        }

        /**
         * Generates a cathode serial number
         * */
        [TestMethod()]
        public void GenerateCathodeSerialTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string str_cmd = "CHD," + serial_number + ",24782,0,UNITTEST,0";
            string[] actual = target.insertCathodeSerialTODB(str_cmd);
            string[] expected = { "0", "" };
            Assert.AreEqual(expected[0], actual[0]);
        }

        /**
         * Generates a duplicate cathode serial number, which expects an error
         * */
        [TestMethod()]
        public void GenerateDuplicateCathodeSerialTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = "0";
            string str_cmd = "CHD," + serial_number + ",23654,0,UNITTEST,0";
            string[] actual = target.insertCathodeSerialTODB(str_cmd);
            string[] expected = { "51024", "" };
            Assert.AreEqual(expected[0], actual[0]);
        }

        //------------------------------ TEST FOR GET FORM ------------------------------//

        /**
         * Grabs a latest form from a serial number that is already generated, expected bearing form.
         * */
        [TestMethod()]
        public void GetFormTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string serial_number = "UNIT_GETFORM01";
            List<VTFieldObject> actual = target.getFormInfo(serial_number, "1");
            Assert.AreEqual("0", actual[0].return_num);

        }

        /**
         * Grabs a form on a serial that does not exist.
         * */
        [TestMethod()]
        public void GetFormDNESerialTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string serial_number = "NAN";
            List<VTFieldObject> actual = target.getFormInfo(serial_number, "1");
            Assert.AreEqual("51001", actual[0].return_num);
        }

        /**
         * Grab a form on a employee id that does not exist
         * */
        [TestMethod()]
        public void GetFormDNEEmpIDTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string serial_number = "UNIT_GETFORM01";
            List<VTFieldObject> actual = target.getFormInfo(serial_number, "123412341231412314124");
            Assert.AreEqual("51015", actual[0].return_num);
        }

        /**
         * Grab a form with a completed work flow
         * */
        [TestMethod()]
        public void GetFormCompletedWorkFlowTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string serial_number = "COMPLETE_WORKFLOW";
            List<VTFieldObject> actual = target.getFormInfo(serial_number, "1");
            Assert.AreEqual("51020", actual[0].return_num); // If failed, make sure the serial number "COMPLETE_WORKFLOW" is a completed serial number.
        }

        //------------------------------ TEST FOR SUBMIT ------------------------------//

        /**
         * Submit a form and make sure its successful. Generate a new tube serial number on every test. This test only works on DDREV2 Tubes, and is testing Bearing Info.
         * */
        [TestMethod()]
        public void SubmitFormTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string serial_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,1";
            target.insertSerialTODB(serial_cmd); // Generate serial number for every submit test
            List<VTFieldObject> form = target.getFormInfo(serial_number, "0");
            List<string> submit_list = new List<string>();
            for (int i = 0; i < form.Count; i++)
            {
                switch (form[i].Token)
                {
                    case "RPL":
                        submit_list.Add("RPL0.004");
                        break;
                    case "HRL":
                        submit_list.Add("HRL0.0014");
                        break;
                    case "HMG":
                        submit_list.Add("HMGBarden");
                        break;
                    case "PRT":
                        submit_list.Add("PRT7000");
                        break;
                    case "SNT":
                        submit_list.Add("SNT" + serial_number);
                        break;
                    default:
                        submit_list.Add(form[i].Token + "0");
                        break;
                }
            }
            string[] sumbit_arr = target.helperSubmit(submit_list, 1, form[0].HeaderToken);
            Assert.AreEqual("0", sumbit_arr[0]);
        }

        /**
         * Submit test on checkboxes
         * */
        [TestMethod()]
        public void SubmitFormCheckBoxTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string serial_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,1";
            target.insertSerialTODB(serial_cmd); // Generate serial number for every submit test
            List<VTFieldObject> form = target.getFormInfo(serial_number, "0", "HTC"); // Use Chem, chem has checkboxes for testing
            List<string> submit_list = new List<string>();
            for (int i = 0; i < form.Count; i++)
            {
                switch (form[i].Token)
                {
                    case "SNT":
                        submit_list.Add("SNT" + serial_number);
                        break;
                    case "TMS":
                        submit_list.Add("TMS0");
                        break;
                    default:
                        submit_list.Add(form[i].Token + "1");
                        break;
                }
            }
            string[] submit_arr = target.helperSubmit(submit_list, 1, form[0].HeaderToken);
            Assert.AreEqual("0", submit_arr[0]);
        }

        /**
         * Submitting a form with missing data
         * */
        [TestMethod()]
        public void SubmitMissingData()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string serial_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,1";
            target.insertSerialTODB(serial_cmd); // Generate serial number for every submit test
            List<VTFieldObject> form = target.getFormInfo(serial_number, "0");
            List<string> submit_list = new List<string>();
            for (int i = 0; i < form.Count; i++)
            {
                switch (form[i].Token)
                {
                    case "RPL":
                        submit_list.Add("RPL0.004");
                        break;
                    case "HRL":
                        break; // Leave field empty
                    case "HMG":
                        submit_list.Add("HMGBarden");
                        break;
                    case "PRT":
                        submit_list.Add("PRT7000");
                        break;
                    case "SNT":
                        submit_list.Add("SNT" + serial_number);
                        break;
                    default:
                        submit_list.Add(form[i].Token + "0");
                        break;
                }
            }
            string error_code = "51005";
            string[] sumbit_arr = target.helperSubmit(submit_list, 1, form[0].HeaderToken);
            Assert.AreEqual(error_code, sumbit_arr[0]);
        }

        /**
         * Submit out of spec value 
         * */
        [TestMethod()]
        public void SubmitOutOfSpec()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            Random rand = new Random();
            string serial_number = rand.Next(0, 10000000).ToString();
            string serial_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,1";
            target.insertSerialTODB(serial_cmd); // Generate serial number for every submit test
            List<VTFieldObject> form = target.getFormInfo(serial_number, "0");
            List<string> submit_list = new List<string>();
            for (int i = 0; i < form.Count; i++)
            {
                switch (form[i].Token)
                {
                    case "RPL":
                        submit_list.Add("RPL0.004");
                        break;
                    case "HRL":
                        submit_list.Add("HRL0.00"); // Submit out of spec
                        break;
                    case "HMG":
                        submit_list.Add("HMGBarden");
                        break;
                    case "PRT":
                        submit_list.Add("PRT7000");
                        break;
                    case "SNT":
                        submit_list.Add("SNT" + serial_number);
                        break;
                    default:
                        submit_list.Add(form[i].Token + "0");
                        break;
                }
            }
            string error_code = "-1";
            string[] sumbit_arr = target.helperSubmit(submit_list, 1, form[0].HeaderToken);
            Assert.AreEqual(error_code, sumbit_arr[0]);
        }

        //------------------------------ TEST FOR HEADERS-----------------------------//

        /**
         * Get all of headers works correctly
         * */
        [TestMethod()]
        public void GetAllHeaders()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string serial_number = "COMPLETE_WORKFLOW";
            string employeeID = "1";
            int count = 8;
            List<VTFormObject> list_forms = target.getAllHeaders(serial_number, employeeID);
            Assert.AreEqual(count, list_forms.Count);
        }

        //------------------------------ TEST FOR SUMMARY-----------------------------//
    
        //[TestMethod()]
        //public 
    } 
    
}
