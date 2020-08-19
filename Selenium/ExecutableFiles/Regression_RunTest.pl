#------------------------------------------------------------------------------------------
# Name:  	     StartupScript.pl
# Parameters:    List of Unit Tests' path to be trigerred
# Environment: 	 Designed for windows environment
# Uses:        	 Double click on StartupScript.pl or under DOS type: perl StartupScript.pl
#	
# Description: 	This program checks for latest build in Build folder and copies the
#               folder to local D Driver and trigerrs the units and emails the report.
#				
# Output:       Console Log and Log file - StartupScript.log, and Email Alerts
# Author:       Automation Team
#-------------------------------------------------------------------------------------------
use POSIX qw(strftime);
use List::Util qw/ max min /;
use XML::Simple qw(:strict);
use Data::Dumper;

my $BuildPath = "\\\\10.4.13.86\\jenkins\\jobs\\iCA_CURR_python\\builds";
my $ControllerName="10.5.38.12";
my $TestDataPath="\\\\$ControllerName\\TestData";
my $ControllerUserName="Administrator";
my $ControllerUserPassword="Pa\$\$word";
my $Domain_User="isgsw\\viptest";
$Domain_User_Data = "Administrator";
my $Password="drama451";
my $Password_Data = "Cedara99";
my $PERSISTENT="\/PERSISTENT\:NO";
my $CurrBuildPath="";
my @currentdaybuilds;
my $Destinationpath;
my $Imgdrvpath = "D:\\BatchExecution\\Selenium\\bin\\Debug\\ExecutableFiles";
my $smtphost="10.5.16.2";
my $currentdate = strftime "%Y-%m-%d", localtime;
#my $currentdate = "2017-03-18";
my @TestResultsfile;
my $Attachements;
my $SenderList = "siva.jawaharji\@aspiresys.com";
#my $SenderList = "lakshman.shiva\@aspiresys.com";
my @Overallresults;
my $len;
my $logiflepath = "StartupScript.log";

##Start logging 
open Log, ">$logiflepath";

### Email services enabled
#Setup SMTP Server
system("$Imgdrvpath\\blat.exe -install $smtphost admin\@merge.com 1 25");    	
    
####### Startup Script #######
MapBuildDirectory();
MapTestDataDirectory();
CheckForNewBuild();
CopyBuild();

#=======================================
#This will trigger the Selenium.exe file
#=======================================
sleep 60;
system ("start D:\\BatchExecution\\Selenium\\bin\\Debug\\Selenium.exe -file D:\\BatchExecution\\Automation_Config.xml");


## Close Log file
print Log GetLoggingTime()."DONE\n";
close Log;

#===============================
#List of Subroutines Starts Here
#===============================

#===
#### This method is to add the shared drive ####
#===
sub MapBuildDirectory
{ 
  print Log GetLoggingTime()."Adding Build Directory\n";	
  print GetLoggingTime()."Adding Build Directory\n";	
  
  my $command_mapdrive = "net USE S: ".$BuildPath." ".$Password." "."\/USER\:$Domain_User $PERSISTENT";
  my $command_removedriver = "net use /delete /y "."S:";
  system($command_removedriver);
  system($command_mapdrive);
  
  if (-e $BuildPath && -d $BuildPath) 
  {
  	print Log GetLoggingTime()."Shared Drive Added--$BuildPath\n";
  	print GetLoggingTime()."Shared Drive Added--$BuildPath\n";
  }
  else
  {
  	print Log GetLoggingTime()."Could not add Shared Drive--$BuildPath\n";
  	print GetLoggingTime()."Could not add Shared Drive-$BuildPath\n";
  	exit;  	
  }
}

#=====
##### This is to Map the Test data directory and to hard link the same
#=====
sub MapTestDataDirectory
{ 
  print Log GetLoggingTime()."Adding TestData Directory\n";	
  print GetLoggingTime()."Adding TestData Directory\n";	
  
  my $command_removedrive = "net use /delete /y "."T:";
  my $REG_command_SetDriveName="reg add HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\MountPoints2\\#\#$ControllerName\#TestData \/v \_LabelFromReg \/t REG\_SZ \/f \/d \"TestData\"";
  my $command_mapdrive = "net use T: $TestDataPath \/USER\:$ControllerUserName $ControllerUserPassword \/PERSISTENT\:yes";
  system($command_removedrive);
  system($REG_command_SetDriveName);
  system($command_mapdrive);
  
  if (-e $TestDataPath && -d $TestDataPath) 
  {
  	print Log GetLoggingTime()."Shared Drive Added--$TestDataPath\n";
  	print GetLoggingTime()."Shared Drive Added--$TestDataPath\n";
  }
  else
  {
  	print Log GetLoggingTime()."Could not add Shared Drive--$TestDataPath\n";
  	print GetLoggingTime()."Could not add Shared Drive-$TestDataPath\n";
  	exit;  	
  }
}

#===
### Check If Latest Build exits and take the path ###
#===
sub CheckForNewBuild
{
my $Builddirectory;
my $count=0;
my %Buildname_timestamp;
my @hours;
my @minutes;
my @seconds;

#Get all Build files in current date
opendir $Builddirectory, $BuildPath;
my @folderlist = readdir $Builddirectory;
print Log GetLoggingTime()."Checking for new build\n";
print GetLoggingTime()."Checking for new build\n";
#my @sortedfolders = @sort { -M -w $1 <=> -M -w $b} @folderlist;
foreach $item (@folderlist)
{
  #print "$item\n";
  #print "date: " stat($item)"\n";
  my $mttime=(stat($item))[9];
  @date=localtime($mttime);
  $date[5]+=1900;
  $date[4]+=1;
  
  #@folder = split(/_/, $item);
  #$length=scalar @folder;

 #check if its a build folder
 if ($item =~ m/([\d]{4})/)
 {
  if($folder[0]eq $currentdate)
  {
    push (@currentdaybuilds, $item);    
  }
 }
}
closedir $Builddirectory;

# Get latest build in the builds directory
opendir(DIR, $BuildPath) or die $!;	
print "List of Builds available:\n";
while (my $file = readdir(DIR))	
{
	# Use a regular expression to ignore files 
	if ($file =~ m/([\d]{4})/)
	{
		print "$file\n";
		$currbuild = $file;
	}
}
print "Latest build mapped : $currbuild\n";
closedir(DIR);

## if mutiple build on same day, take latest one
$len = scalar(@currentdaybuilds);
if($len>1)
{   
  $CurrBuildPath = $currentdaybuilds[$len-1];
  print GetLoggingTime()."There are multiple builds taking the build--.$CurrBuildPath";
  print Log GetLoggingTime()."There are multiple builds taking the build--.$CurrBuildPath";  
 }
 elsif($len==1)
 {
  $CurrBuildPath = $currentdaybuilds[0];  
  print GetLoggingTime()."There is only one new build for the day--".$CurrBuildPath;
  print Log GetLoggingTime()."There is only one new build for the day--".$CurrBuildPath;
 }
 elsif($currbuild!="")
 {
  $CurrBuildPath = $currbuild;  
  print GetLoggingTime()."There is only one new build for the day--".$CurrBuildPath;
  print Log GetLoggingTime()."There is only one new build for the day--".$CurrBuildPath;
 }
 else
 {
 	print GetLoggingTime()."There are no new builds for the day\n";
 	print Log GetLoggingTime()."There are no new builds for the day\n";
 	my $emailbody = "BuildNotFound.txt";
 	open $fh1, '>', $emailbody;
 	print $fh1 "Hi All,\n\n";
 	print $fh1 "There is no new build trigerred for Today. Automation Batch Terminated\n\n";
 	print $fh1 "Thanks.\n";
 	print $fh1 "AutomationTeam.\n\n";
 	close $fh1;
 	
	#Send mail to team
 	$commandsendemail = "$Imgdrvpath\\blat $emailbody -to $SenderList -subject ICA_NoBuildFound -log";
    system($commandsendemail);
 	exit;
 }
}

#===
#### Copy the build to detination folder ####
#===
sub CopyBuild
{  
  my $SourcePath = $BuildPath."\\".$CurrBuildPath;
  $Destinationpath = "D:\\WebAccess_build"."\\".$CurrBuildPath."\\*\.*";
  my $copycommand  = "xcopy $SourcePath $Destinationpath  \/s \/d \/r \/y";
  print GetLoggingTime()."Copy command is $copycommand\n";
  print Log GetLoggingTime()."Copy command is $copycommand\n";
  system($copycommand);  
  sleep 10;
  my $command_removedriver = "net use /delete /y "."S:";
  system($command_removedriver);    
}


#=== 
#This will get the current date timepstam
#===
 sub GetLoggingTime
 {

    my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst)=localtime(time);
    my $nice_timestamp = sprintf ("%04d%02d%02d %02d:%02d:%02d",$year+1900,$mon+1,$mday,$hour,$min,$sec);
    return $nice_timestamp."-";
}
