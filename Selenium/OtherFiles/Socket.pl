use strict;

use IO::Socket::INET;
$| = 1;

sleep(30);

my $socket = new IO::Socket::INET (
    LocalHost => '0.0.0.0',
    LocalPort => '7777',
    Proto => 'tcp',
    Listen => 5,
    Reuse => 1
);
die "cannot create socket $!\n" unless $socket;

while(1)
{
    # waiting for a new client connection
	print "server waiting for client connection on port 7777\n";
    my $client_socket = $socket->accept();
 
    # get information about a newly connected client
    my $client_address = $client_socket->peerhost();
    my $client_port = $client_socket->peerport();
    print "connection from $client_address:$client_port\n";
 
    # read up to 1024 characters from the connected client
    my $recieveData = "";
    $client_socket->recv($recieveData, 1024);
    print "received data: $recieveData\n";
 
    # write response data to the connected client
    my $data = "1001";
	#$client_socket->send(length($data));
    $client_socket->send($data);
	
	print "Sent Data : $data\n";
	
    # notify client that response has been sent
	$client_socket->recv($recieveData, 1024);
	if ($recieveData != 0)
	{
		if($recieveData == 2019)
		{
			&RestartICAService();
			sleep(20);
			$client_socket->send($data);
			print "Sent Data : $data\n";
		}
	}
	else
	{
		print "Unknown data: $data\n";
	}
}

sub RestartICAService()
{
	print "Going to Restart ICA Service \n";
	
	my @rc = `net stop W3SVC`;
	if($rc[1]=~ /success/)
	{
		print "W3SVC service stopped successfully \n";
	}
	else
	{
		print "Error occured while stopping W3SVC \n"
	}
	sleep(20);
	
	@rc = `net stop DRAppGateManager`;	
	if ($rc[1] =~ /success/)
	{
		print "DRAppGateManager stopped successfully\n";
	}
	else
	{
		print "Problem occurred stopping DRAppGateManager\n";
	}
	sleep(20);
	
	@rc = `net start DRAppGateManager`;
	if($rc[1] =~ /success/ )
	{
		print "DRAppGateManager started successfully\n";
	}
	else
	{
		print "Problem aoccured while restarting DRAppGateManager\n";
	}	
	sleep(20);
	
	@rc = `net start W3SVC`;
	if($rc[1] =~ /success/)
	{
		print "W3SVC Service started successfully \n";
	}
	else
	{
		print "Error occured while staring W3SVC Service \n";
	}
	sleep(20);
}

$socket->close();

