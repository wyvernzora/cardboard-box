<?php
include("conn.php");
$query = explode(" ",$_POST['query']);
foreach($query as $tag) {
	//echo "Inserting ".$tag."<br>";
	mysql_query("INSERT INTO log (tag) VALUES ('$tag')") or die ("ERROR_INSERT_LOG");
	$q = "SELECT * FROM tag WHERE tag='$tag'";
	$result = mysql_query($q);
	if(mysql_num_rows($result) > 0) { //Increment the count
		$row = mysql_fetch_assoc($result);
		$count = $row['ref'];
		$count++;
		$q = "UPDATE tag SET ref='$count' WHERE tag='$tag'";
		mysql_query($q) or die("ERROR_UPDATE");
	}
	else { //Insert a new entry with count 1
		$date = date("Y-m-d H:i:s");
		$q = "INSERT INTO tag (tag,ref) VALUES ('$tag','1')";
		mysql_query($q) or die("ERROR_INSERT");
	}
}
echo "OK";
?>