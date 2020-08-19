use File::Copy;
sub readFileGeneric {
	my $ifilename=shift;
	my $odata="";
	open(read1,"$ifilename");
	while(<read1>) {
		$odata.=$_;
	}
	close(read1);
	return($odata);
}
sub readXML {
	my $aa=shift;
	my $bb=shift;
	my $c1="</$aa>";
	my $c2="<$aa>";
	my @cc=split($c1,$bb);
	my @dd=split($c2,$cc[0]);
	my $ee=$dd[1];
	return($ee);
}

my $filename=$ARGV[0];
my $filename2=$ARGV[1];
$tempfile = $filename2."\\packages_old.config";
$oldfile = $filename2."\\packages.config";
print "Argument is $filename \n";
print "Argument2 is $filename2 \n";

if( -e $filename) {
my $wholefilecontent=&readFileGeneric($filename);
 $Browser=&readXML("Name",$wholefilecontent);
}
print "Browser Name $Browser \n";
if($Browser eq "chrome" || $Browser eq "firefox" || $Browser eq "firefoxz3d") {
print "Chrome Block Executed";
$newFile = $filename2."\\packages_chrome.config";
#copy("sourcefile","destinationfile") or die "Copy failed: $!";
rename $oldfile, $tempfile or die "Cannot rename file: $!";
rename $newFile, $oldfile or die "Cannot rename file: $!";
}
else
{
print "Ie Block Executed";
$newFile1 = $filename2."\\packages_ie.config";
rename $oldfile, $tempfile or die "Cannot rename file: $!";
rename $newFile1, $oldfile or die "Cannot rename file: $!";
}
