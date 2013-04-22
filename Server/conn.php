 <?php 
$link = mysql_connect('devfishorg.fatcowmysql.com', 'db_owner', 't9fr3dRe'); 
if (!$link) { 
    die('SERVER_ERROR'); 
} 
//echo 'Connected successfully'; 
mysql_select_db(opix); 
?> 