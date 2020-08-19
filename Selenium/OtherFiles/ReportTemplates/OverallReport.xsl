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
<table align="center" border="0" cellpadding="4" cellspacing="0" style="border:0px solid" width="100%">
       <tr>
          <td align="left"><img border="0" src="ReportTemplates/logo_merge_healthcare.png" /></td>   
	  <!--td align="right" valign = "bottom"><img border="0" src="XmlReference/ASP_Logo.jpg" /></td-->
       </tr>
	   <td><hr/></td>
</table>

<table border="0" width="100%" style="color:#003399">
<tr>
<td><h3><font face="Verdana">Overall Report Summary</font></h3></td>
<td align="right" valign="top"><b>Build Version :</b> <xsl:value-of select="XMLStorage/Summary/BuildVersion"/></td>
</tr>
</table>

<xsl:variable name="PassCount" select="sum(/XMLStorage/Modules/Module/Pass)"/>
<xsl:variable name="WarningCount" select="sum(/XMLStorage/Modules/Module/NotExecuted)"/>
<xsl:variable name="FailCount" select="sum(/XMLStorage/Modules/Module/Fail)"/>

<xsl:variable name="passwidth" select="round(($PassCount) div ($PassCount+$FailCount) * 100)" />
<xsl:variable name="failwidth" select="round($FailCount div ($PassCount+$FailCount) * 100)" />
<!-- Draw the horizontal bar -->


<table width="45%">
<tr>
<td>

<table border="0" cellpadding="2" cellspacing="2" width="100%" style="border:1px solid;color:#003399">
   <tr><td style="color:003399" width = "50%">Total no. of Test Cases executed </td><td>:</td><td  style="color:003399;font-weight:bold;"><xsl:value-of select="($PassCount+$FailCount)"/></td></tr>
   <tr><td></td><td></td><td></td></tr>
   <tr><td style="color:003399">Total no. of Test Cases Passed</td><td>:</td><td style="color:green; font-weight:bold;"><xsl:value-of select="($PassCount)"/> </td></tr>
   <tr><td></td><td></td><td></td></tr>
   <tr><td style="color:003399">Total no. of Test Cases Failed</td><td>:</td><td style="color:red; font-weight:bold;"><xsl:value-of select="$FailCount"/> </td></tr>
   
   <tr><td></td><td></td><td></td></tr>	
   <tr><td></td><td></td><td></td></tr>	
   <tr><td></td><td></td><td></td></tr>	 
   <tr><td></td><td></td><td></td></tr>	

   <tr><td style="color:003399">% of Test Cases Passed</td><td>:</td>
       <td align="left">
          <div style="background-color:green;width:{$passwidth}%;text-align:center"><font color="black"><b><xsl:value-of select="$passwidth" />%</b></font></div>
       </td> 
   </tr>    
   <tr><td></td><td></td><td></td></tr>	
	
   <tr><td style="color:003399">% of Test Cases Failed</td><td>:</td>
       <td align="left">
          <div style="background-color:red;width:{$failwidth}%;text-align:center"><font color="black"><b><xsl:value-of select="$failwidth" />%</b></font> </div>
       </td>
   </tr>    
   <tr><td/><td/><td/></tr>	 

</table>
</td>
</tr>

</table>

<br/><hr/> 

<!--<table border="0" width="100%" align="center">-->
<!--<tr><td>-->
<div style="float: left;width : 50%">
<table border="0" width="100%" style="color:#003399;float: left">
<tr>
<td align="left" valign="bottom" style="color:#003399"><h3><font face="Verdana">Overall Report Details</font></h3></td></tr>
<tr><td></td></tr>
<tr><td align="left" valign="bottom"><b>Server Machine Name  :  </b> <xsl:value-of select="XMLStorage/Summary/ServerName"/> </td></tr>
<tr><td></td></tr>
<tr><td align="left" valign="bottom"><b>Client Machine Name  :  </b> <xsl:value-of select="XMLStorage/Summary/ClientName"/> </td></tr>
<tr><td></td></tr>
<td align="left" valign="bottom"><b>Browser     		 :  </b> <xsl:value-of select="XMLStorage/Summary/BrowserType"/> </td>
<tr><td></td></tr>
<tr><td align="left" valign="bottom"><b>Operating System     :  </b> <xsl:value-of select="XMLStorage/Summary/OS"/> </td></tr>
<tr><td></td></tr>
<tr><td align="left" valign="bottom"><b>Total Execution Time :  </b> <xsl:value-of select="XMLStorage/Summary/TotalExecutionTime"/> </td></tr>
<tr><td></td></tr>
<tr><td align="left" valign="bottom"><b>Executed on  		:  </b> <xsl:value-of select="XMLStorage/Summary/ExecutionTime"/> </td></tr>
</table>
<!--</td><td align="right">-->
</div>
<div style="float: left;width : 50%">
<table border="0" width="100%" style="color:#003399;float: left">
	<tr><td>
		<xsl:if test="XMLStorage/AdditionalServers/AdditionalServer">
			<!--<div style="float: left">-->
				<table border="0" width="100%" style="color:#003399">
					<tr>
<td align="left" valign="bottom" style="color:#003399"><h3><font face="Verdana">Additional Server(s)</font></h3></td></tr>
					<xsl:for-each select="XMLStorage/AdditionalServers/AdditionalServer">
						<tr>
							<td	align="Left" valign="top"><font color="#ff6600"><b>	<xsl:value-of select="@Type"/> : </b></font> <xsl:value-of select="@MachineName"/> (<xsl:value-of select="@MachineIP"/>). Version - <xsl:value-of select="@Version"/>
							</td>
						</tr>
						<tr><td></td></tr>
					</xsl:for-each>
				</table>
			<!--</div>-->
		</xsl:if>
	</td></tr>
</table>
<!--</td></tr>
</table>-->
</div>
<br/>
<!--<table align="right" style="color:#003399">
<tr><td valign="bottom"><font color="#ff6600"><b>Executed on  		:  </b></font> <xsl:value-of select="XMLStorage/Summary/ExecuteTime"/></td></tr>
</table>-->
<br/>

<table border="0" width="100%" cellpadding="6" cellspacing="1" style="border:1px solid;">
   <tr bgcolor="#003399">
      <th style="color:white">S.No</th>
	  <th style="color:white">Module Name</th>
	  <th style="color:white">Total Cases</th>
      <th style="color:white">Pass Case</th>
	  <th style="color:white">Fail Case</th>
      <th style="color:white">Status</th>
	  <th style="color:white">Execution Time</th>
	  <th style="color:white">Detailed Result</th>
    </tr>

   <xsl:for-each select="XMLStorage/Modules/Module">   
    <tr>
      <td width="5%" align="center"><xsl:value-of select="position()"/></td>      

      <xsl:choose>
		<xsl:when test="Status ='Fail'">
           <td width="20%" align="left" style="color:red;"><xsl:value-of select="@Name"/></td>
        </xsl:when>
		<xsl:when test="Status ='WARNING'">
           <td width="20%" align="left" style="color:#ff9933;"><xsl:value-of select="@Name"/></td>
        </xsl:when>
		<xsl:otherwise>
			<td width="20%" align="left"><xsl:value-of select="@Name"/></td> 
        </xsl:otherwise>        
      </xsl:choose>    

	  <td width="10%" align="center"><xsl:value-of select="TotalTestCases"/></td>
	  <td width="10%" align="center"><xsl:value-of select="Pass"/></td>
	  <td width="10%" align="center"><xsl:value-of select="Fail"/></td>
	  
	  <xsl:choose>
		<xsl:when test="Result ='Pass'">
           <td width="10%" align="center" style="color:green;"><xsl:value-of select="Result"/></td>
        </xsl:when>
		<xsl:when test="Result ='WARNING'">
           <td width="10%" align="center" style="color:#ff9933;"><xsl:value-of select="Result"/></td>
        </xsl:when>
		<xsl:otherwise>
			<td width="10%" align="center" style="color:red;"><xsl:value-of select="Result"/></td> 
        </xsl:otherwise>        
      </xsl:choose>
	  
	  <td width="10%" align="center"><xsl:value-of select="@Duration"/></td>
        <td width="10%" align="center">
	   		<a target="_blank" href="{DetailedView}">
				<xsl:value-of select="DetailedView/@name"/>
			</a>
		</td>
      
    </tr>
   
   </xsl:for-each>
   <tr>
	<td/>
		<td/>
	<td align="right"><b> Total Count  </b></td>
	<td align="center"> <xsl:value-of select="sum(/XMLStorage/Modules/Module/TotalTestCases)"/> </td>
	<td align="center"> <xsl:value-of select="sum(/XMLStorage/Modules/Module/Pass)"/> </td>
	<td align="center"> <xsl:value-of select="sum(/XMLStorage/Modules/Module/Fail)"/> </td>
	<td>  </td>
	<td>  </td>
   </tr>
   
</table>

</body>
</html>
</xsl:template>
</xsl:stylesheet>