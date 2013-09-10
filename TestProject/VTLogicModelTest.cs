using VT.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace TestProject
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


        /// <summary>
        ///Genearte Serial Number Test
        ///</summary>
        [TestMethod()]
        public void GenerateTubeSerialTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor(); 
            
            string serial_number = Console.ReadKey().ToString();
            string str_cmd = "SNT," + serial_number + ",24782,0,UNITTEST,0";
            
            string[] actual = target.insertSerialTODB(str_cmd);
            string[] expected = { "0", "" };
            Assert.AreEqual(expected[0], actual[0]);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void GetFormTest()
        {
            VTLogicModel_Accessor target = new VTLogicModel_Accessor();
            string sn = "UNIT_GETFORM01";
            //List<VTFieldObject> actaul = target.getFormInfo

        }

    }
}
