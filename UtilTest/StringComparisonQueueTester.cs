using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.IO;

namespace UtilTest
{
    [TestClass]
    public class StringComparisonQueueTester
    {
        private string str = @"<FileOptions>
	<FlexFlat name='Thomson Reuters - GL1025'>
		<Split widthType='Separator' separator='&#10;' ignore='&#13;'>
			<Select name='DetailBillingDataRecord' column='1' width='1' value='1'>
				<!-- Following line duplicates existing GL1025 behavior but seems to be blank in sample files -->
				<Edit name='OriginatingCmAccountNumber' column='183' width='20'  fieldType='Fixed' action='tokenize'/>
				<!-- Following line duplicates existing GL1025 behavior and present in sample files -->
				<Edit name='BillingAccountNumber' column='208' width='20'  fieldType='Fixed' action='tokenize'/>
				<!-- 
					Base Account ID subfield within Transaction ID field. 
					Mismatch between spec and sample file; spec calls out the following starting at offset 632
					within the record:
						Business Process Julian Date(5),       
						Global Client Origin Identifier(15),  
						Base Acct ID(11),                    
						Base Supp No.(2)                   
						Batch Number(3)                    
						Transaction Sequence Number(7)
					This corresponds to a 'Base Acct ID' starting column of 632 + 5 + 15 = 652. However the sample
					file provided by Thomson Reuters has the masked 11-char field starting at column 646.
					
					Sample file trumps specification, so the edit element below uses 646 as its offset.
					
					If it's determined that we should be masking instead of tokenizing, use action='fill'. The fill
					character will be the 'bad value' character, configurable as the 'Fill bad values with' field
					in the merchant configuration.
				-->
				<Edit name='BaseAcctId' column='646' width='11'  fieldType='Fixed' action='fill'/>
			</Select>
			<Select name='CmTransactionSummary' column='1' width='1' value='2'>
				<!-- Duplicates existing GL1025 behavior but not called out specifically in TR requirements -->
				<Edit name='CardmemberAccountNumber' column='61' width='20'  fieldType='Fixed' action='tokenize'/>				
				<Edit name='OriginalCardmemberNumber' column='189' width='11'  fieldType='Fixed' action='fill'/>
			</Select>
		</Split>
	</FlexFlat>
</FileOptions>";

        [TestMethod]
        public void CanFindPatternInString()
        {
            var q = new StringComparisonQueue("</Select>");
            foreach (char c in str)
            {
                if (q.FeedAndCompare(c))
                    break;
            }
            Assert.AreEqual(1707,q.Index);
        }
    }
}