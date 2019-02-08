function cnvs_name(id){
	var name = "";
	
	/* servant */
	if(id > '10001' && id < '10037'){ name = "warrior";}
	if(id > '10036' && id < '10073'){ name = "thief";}
	if(id > '10072' && id < '10109'){ name = "cleric";}
	
	/* monster */
	if(id == '20001'){ name = "Panda";}
	if(id == '20002'){ name = "Emperor Panda";}
	if(id == '20003'){ name = "Anubis";}
	if(id == '20004'){ name = "Urs";}
	if(id == '20005'){ name = "Ursus";}
	if(id == '20006'){ name = "Ursa";}
	if(id == '20007'){ name = "Usia";}
	if(id == '20008'){ name = "Griffon";}
	if(id == '20009'){ name = "Black Spade";}
	if(id == '20010'){ name = "Red Heart";}
	if(id == '20011'){ name = "Green Club";}
	if(id == '20012'){ name = "Yellow Diamond";}
	if(id == '20013'){ name = "Nero";}
	if(id == '20014'){ name = "Sharo";}
	if(id == '20015'){ name = "Haro";}
	if(id == '20016'){ name = "Broken";}
	if(id == '20017'){ name = "Kibra";}
	if(id == '20018'){ name = "Deathscythe";}
	if(id == '20019'){ name = "Eggtang";}
	if(id == '20020'){ name = "Eggpong";}
	if(id == '20021'){ name = "Egglue";}
	if(id == '20022'){ name = "Salamander";}
	if(id == '20023'){ name = "Gnome";}
	if(id == '20024'){ name = "Undine";}
	if(id == '20025'){ name = "Gangsi";}
	if(id == '20026'){ name = "Moss Golem";}
	if(id == '20027'){ name = "Lava Golem";}
	if(id == '20028'){ name = "Puni";}
	if(id == '20029'){ name = "Boni";}
	if(id == '20030'){ name = "Hani";}
	if(id == '20031'){ name = "Phainos";}
	if(id == '20032'){ name = "Tarok";}
	if(id == '20033'){ name = "G'rul";}
	if(id == '20034'){ name = "Bobon";}
	if(id == '20035'){ name = "Gund";}
	if(id == '20036'){ name = "Croll";}
	if(id == '20037'){ name = "Sphatoi";}
	if(id == '20038'){ name = "Argo";}
	if(id == '20039'){ name = "Bardolf";}
	if(id == '20040'){ name = "Zombie";}
	if(id == '20041'){ name = "Gaboster";}
	if(id == '20042'){ name = "Sharktooth";}
    if(id == '20043'){ name = "Noahtle";}
    if(id == '20044'){ name = "Succubus";}    
    if(id == '20045'){ name = "";}
    if(id == '20046'){ name = "";}
    if(id == '20047'){ name = "";}
    if(id == '20048'){ name = "";}
    if(id == '20049'){ name = "";}
    if(id == '20050'){ name = "";}
    
    /* item */
    if(id == '30001'){name = "Novice Blunt";}
    if(id == '30002'){name = "Novice Dagger";}
    if(id == '30003'){name = "Novice Earing";}
    if(id == '30004'){name = "Novice Heavy Armor";}
    if(id == '30005'){name = "Novice Light Armor";}
    if(id == '30006'){name = "Novice Necklace";}
    if(id == '30007'){name = "Novice Ring";}
    if(id == '30008'){name = "Novice Robe";}
    if(id == '30009'){name = "Novice Sword";}
    if(id == '30010'){name = "Blunt of Forest";}
    if(id == '30011'){name = "Dagger of Forest";}
    if(id == '30012'){name = "Earing of Forest";}
    if(id == '30013'){name = "Heavy Armor of Forest";}
    if(id == '30014'){name = "Light Armor of Forest";}
    if(id == '30015'){name = "Necklace of Forest";}
    if(id == '30016'){name = "Ring of Forest";}
    if(id == '30017'){name = "Robe of Forest";}
    if(id == '30018'){name = "Sword of Forest";}
    if(id == '30019'){name = "Forsaken Blunt";}
    if(id == '30020'){name = "Forsaken Dagger";}
    if(id == '30021'){name = "Forsaken Earing";}
    if(id == '30022'){name = "Forsaken Heavy Armor";}
    if(id == '30023'){name = "Forsaken Light Armor";}
    if(id == '30024'){name = "Forsaken Necklace";}
    if(id == '30025'){name = "Forsaken Ring";}
    if(id == '30026'){name = "Forsaken Robe";}
    if(id == '30027'){name = "Forsaken Sword";}
    if(id == '30028'){name = "Royal Blunt";}
    if(id == '30029'){name = "Royal Dagger";}
    if(id == '30030'){name = "Royal Earing";}
    if(id == '30031'){name = "Royal Heavy Armor";}
    if(id == '30032'){name = "Royal Light Armor";}
    if(id == '30033'){name = "Royal Necklace";}
    if(id == '30034'){name = "Royal Ring";}
    if(id == '30035'){name = "Royal Robe";}
    if(id == '30036'){name = "Royal Sword";}
    
    return name;
}

function cnvs_grade(grade){
	var grade_name = "";
	
	/* grade */
	if(grade == '1'){grade_name = "Legendary";}
	if(grade == '2'){grade_name = "Unique";}
	if(grade == '3'){grade_name = "Rare";}
	if(grade == '4'){grade_name = "Uncommon";}
	if(grade == '5'){grade_name = "Common";}

	return grade_name;
}

function cnvs_rank(basic){
	var rank = "";
	
	if(basic == '0'){rank = "D";}
	if(basic == '1'){rank = "D+";}
	if(basic == '2'){rank = "C";}
	if(basic == '3'){rank = "C+";}
	if(basic == '4'){rank = "B";}
	if(basic == '5'){rank = "B+";}
	if(basic == '6'){rank = "A";}
	if(basic == '7'){rank = "A+";}
	if(basic == '8'){rank = "S";}
	if(basic == '9'){rank = "S+";}
	
	return rank;
}