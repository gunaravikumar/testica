<?xml version="1.0" encoding="ISO-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">

<html>
<head>
<style type="text/css">
	Table{font-family: Verdana; font-size: 10pt;}
</style>
</head>

<body>
<table align="center" border="0" cellpadding="5" cellspacing="0" style="border:0px solid" width="100%">
       <tr>
          <td align="left"><img border="0" src="../ReportTemplates/logo_merge_healthcare.png" /></td>
	  <!--td align="right" valign = "bottom"><img border="0" src="../XmlReference/ASP_Logo.jpg" /></td-->           
       </tr>
	   <td><hr /></td>
</table>

<table align="center" border="0" width="100%" cellpadding="5" cellspacing="0">
<tr style="color:#003399">
<td align="left"><a name="#top"><h3><font face="Verdana">Detailed Summary</font></h3></a></td>
<td align="right"><b>Build Version : </b> <xsl:value-of select="XMLStorage/Modules/Version"/><br/>
<b>Executed on : </b> <xsl:value-of select="XMLStorage/Modules/TestCase/@StartTime"/> </td>
</tr>
</table>
<br/>

<table border="0" width="90%" cellpadding="5" cellspacing="1" style="border:1px solid">
   <tr bgcolor="#003399">
      <th width="10px" style="color:white">S.No</th>
	  <th width="18px" style="color:white">QAC ID</th>
      <th style="color:white">TestCase Name</th>
	  <th style="color:white">TestData</th>
      <th style="color:white">Status</th>
	  <th style="color:white">Total Steps</th>
	  <th style="color:white">Pass</th>
	  <th style="color:white">Fail</th>
	<th style="color:white">Skip</th>
	  <th style="color:white">Execution Time</th>
	  <th style="color:white">View</th>
    </tr>

<xsl:for-each select="XMLStorage/Modules/TestCase">
	
	<xsl:variable name="testcase" select="@Name"/>
	<xsl:variable name="snoid" select="position()"/>
	<xsl:variable name="QACId" select="@ID"/>
	
	
	<xsl:for-each select="TestData">
	<tr>
	<td width="3%" align="center" style="background-color:CCCCFF;"><xsl:value-of select="$snoid"/></td>
	<td width="5%" align="center" style="background-color:CCCCFF;"><xsl:value-of select="$QACId"/></td>
	
	<td width="30%" style="background-color:CCCCFF;"><xsl:value-of select="$testcase"/></td>
		<td width="26%" style="background-color:CCCCFF;"><xsl:value-of select="Data"/></td>
		
		<xsl:choose>
			<xsl:when test="count(Steps/Step[normalize-space(Result)='FAIL']) > 0">
			<td width="10%" align="center" style="background-color:CCCCFF;color:red;" ><b>FAIL</b></td>
			</xsl:when>
			<xsl:otherwise>
			<td width="10%" align="center" style="background-color:CCCCFF;color:green"><b>PASS</b></td>
			</xsl:otherwise>
		</xsl:choose>
		<td width="13%" style="background-color:CCCCFF;" align="center"><xsl:value-of select="count(Steps/Step)"/></td>
		<td width="7%" style="background-color:CCCCFF;" align="center"><xsl:value-of select="count(Steps/Step[normalize-space(Result)='PASS'])"/></td>
		<td width="7%" style="background-color:CCCCFF;" align="center"><xsl:value-of select="count(Steps/Step[normalize-space(Result)='FAIL'])"/></td>
		<td width="7%" style="background-color:CCCCFF;" align="center"><xsl:value-of select="count(Steps/Step[normalize-space(Result)='SKIP'])"/></td>
		<td width="14%" style="background-color:CCCCFF;" align="center"><xsl:value-of select="@Duration"/></td>
	
		<xsl:variable name="Linkname" select="1 + 1"/>
		<td align="center" style="background-color:CCCCFF;">
		<a href="#{$testcase}">View</a>
		
		</td>
	   	</tr>
	</xsl:for-each>

</xsl:for-each>
    <tr>
	<td/>
	<td/>
	<td/>
	<td/>
	<td align="right"><b>Total Count</b></td>
	<td align="center"> <xsl:value-of select="count(/XMLStorage/Modules/TestCase/TestData/Steps/Step)"/> </td>
	<td align="center"> <xsl:value-of select="count(/XMLStorage/Modules/TestCase/TestData/Steps/Step[normalize-space(Result)='PASS'])"/> </td>
	<td align="center"> <xsl:value-of select="count(/XMLStorage/Modules/TestCase/TestData/Steps/Step[normalize-space(Result)='FAIL'])"/> </td>
	<td align="center"> <xsl:value-of select="count(/XMLStorage/Modules/TestCase/TestData/Steps/Step[normalize-space(Result)='SKIP'])"/> </td>
	<td>  </td>
	<td>  </td>
   </tr>
</table>
<br/>

<table align="center" border="0" width="100%" cellpadding="5" cellspacing="0">
<tr style="color:#003399">
<td align="left"><h3><font face="Verdana">Detailed Report</font></h3></td>
</tr>
</table>

<xsl:for-each select="XMLStorage/Modules/TestCase">
<xsl:variable name="testcasename" select="@Name"/>
<xsl:variable name="QACId" select="@ID"/>
<xsl:for-each select="TestData">
<br/>
<table align="center" border="0" width="99%">
<tr>
<td style="color:#660066;">
<font face="Verdana"><b>TestCase Name : </b>
<label><xsl:value-of select="$QACId"/> - </label>
<a name="{$testcasename}"><xsl:value-of select="$testcasename"/></a>
</font>
</td>
</tr>
<tr>
<td style="color:#660066;">
<font face="Verdana"><b>Test Data : </b>
<a><xsl:value-of select="Data"/></a>
</font>
</td>
</tr>
</table>

<table border="0" width="97%" cellpadding="5" cellspacing="1" style="border:1px solid border-color:003399" align="center">
   <tr bgcolor="9999FF">
      <th style="color:white">S.No</th>
      <th style="color:white">TestStep</th>
      <th style="color:white">Expected Result</th>
	  <th style="color:white">Actual Result</th>
      <th style="color:white">Result</th>
	  <th style="color:white">Comments</th>
	  <th style="color:white">Screenshot</th>
    </tr>
	
	<xsl:for-each select="Steps/Step">
    <tr>
      <xsl:choose>
		<xsl:when test="Result ='FAIL'">
			<td width="5%" align="center"><xsl:value-of select="position()"/></td>
           <td width="30%" align="left" style="color:red;"><xsl:value-of select="StepsSummary"/></td>
		   <td width="40%" align="left" style="color:red;"><xsl:value-of select="ExpectedResult"/></td>
		   <td width="40%" align="left" style="color:red;"><xsl:value-of select="ActualResult"/></td> 
        </xsl:when>
        <xsl:otherwise>
           <td width="5%" align="center"><xsl:value-of select="position()"/></td>
		   <td width="30%" align="left"><xsl:value-of select="StepsSummary"/></td>
		   <td width="40%" align="left"><xsl:value-of select="ExpectedResult"/></td> 
		   <td width="40%" align="left" style="color:green;"><xsl:value-of select="ActualResult"/></td> 
        </xsl:otherwise>
      </xsl:choose> 
	  
      <xsl:choose>
		<xsl:when test="Result ='PASS'">
           <td width="10%" align="center" style="color:green;"><b><xsl:value-of select="Result"/></b></td>
			
		</xsl:when>
        <xsl:when test="Result ='ALERT'">
           <td width="10%" align="center" style="color:#ff9933;"><b><xsl:value-of select="Result"/></b></td>
		</xsl:when>
        
        <xsl:when test="Result ='WARNING'">
          <td width="10%" align="center" style="color:#ff9933;">
            <b>
              <xsl:value-of select="Result"/>
            </b>
          </td>
        </xsl:when>
        <xsl:when test="Result ='INFO'">
          <td width="10%" align="center" style="color:blue;">
            <b>
              <xsl:value-of select="Result"/>
            </b>
          </td>
        </xsl:when>
		<xsl:otherwise>
	   <td width="10%" align="center" style="color:red;"><b><xsl:value-of select="Result"/></b></td> 
        </xsl:otherwise>
      </xsl:choose>
	  
	  <td width="15%" align="left">
		<xsl:value-of select="Comments"/>
	  </td>
	 <xsl:choose>
		<xsl:when test="Screenshot='' and GoldImage=''">
			<td/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:choose>
				<xsl:when test="not(Screenshot='')">
				<td width="15%" align="center">
					<a target="_blank" href="{concat('..\..\',Screenshot)}">ErrorImage</a><br/>
					<xsl:choose>
						<xsl:when test="not(GoldImage='')">
							<a target="_blank" href="{concat('..\..\',GoldImage)}">GoldImage</a><br/>
							<a target="_blank" href="{concat('..\..\',TestImage)}">TestImage</a>
							<xsl:choose>
								<xsl:when test="not(DiffImage='')">
									<br/><a target="_blank" href="{concat('..\..\',DiffImage)}">
									DiffImage
									</a>
								</xsl:when>
							</xsl:choose>
						</xsl:when>
					</xsl:choose>
					
				</td>
				</xsl:when>
				<xsl:otherwise>
					<td width="15%" align="center">
						<a target="_blank" href="{concat('..\..\',GoldImage)}">
						GoldImage
						</a>
						<br/>
						<a target="_blank" href="{concat('..\..\',TestImage)}">
						TestImage
						</a>
						<xsl:choose>
							<xsl:when test="not(DiffImage='')">
								<a target="_blank" href="{concat('..\..\',DiffImage)}">
								DiffImage
								</a>
							</xsl:when>
						</xsl:choose>
					</td>
				</xsl:otherwise>
			</xsl:choose>

		</xsl:otherwise>
	  </xsl:choose>	  
    </tr>
   </xsl:for-each>
   
</table>

</xsl:for-each>


<table border="0" width="100%" cellpadding="5" cellspacing="0">
<td align="right"><a href="#top"><font face="Verdana"> Top </font></a></td>
</table>
<br/>
<hr/>
</xsl:for-each>

</body>
</html>
</xsl:template>
</xsl:stylesheet>