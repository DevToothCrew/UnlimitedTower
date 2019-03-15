function cnvs_name(id){
	var name = "";
	
	/* servant */
	if(id > '10000' && id < '10037'){ name = "Warrior";}
	if(id > '10036' && id < '10073'){ name = "Thief";}
	if(id > '10072' && id < '10109'){ name = "Cleric";}
	
	/* monster */
	if(id == '20001'){name = "Panda";}
	if(id == '20002'){name = "Emperor Panda";}
	if(id == '20003'){name = "Anubis";}
	if(id == '20004'){name = "Ranubis";}
	if(id == '20005'){name = "Urs";}
	if(id == '20006'){name = "Ursus";}
	if(id == '20007'){name = "Ursa";}
	if(id == '20008'){name = "Usia";}
	if(id == '20009'){name = "Griffon ";}
	if(id == '20010'){name = "Luke";}
	if(id == '20011'){name = "Black Spade";}
	if(id == '20012'){name = "Red Heart";}
	if(id == '20013'){name = "Green Club";}
	if(id == '20014'){name = "Yellow Diamond";}
	if(id == '20015'){name = "Nero";}
	if(id == '20016'){name = "Sharo";}
	if(id == '20017'){name = "Haro";}
	if(id == '20018'){name = "Broken";}
	if(id == '20019'){name = "Kibra";}
	if(id == '20020'){name = "Deathscythe";}
	if(id == '20021'){name = "Spectre";}
	if(id == '20022'){name = "Eggtang";}
	if(id == '20023'){name = "Eggpong";}
	if(id == '20024'){name = "Egglue";}
	if(id == '20025'){name = "Salamander";}
	if(id == '20026'){name = "Gnome";}
	if(id == '20027'){name = "Undine";}
	if(id == '20028'){name = "Gangsi";}
	if(id == '20029'){name = "Moss Golem";}
	if(id == '20030'){name = "Lava Golem";}
	if(id == '20031'){name = "Puni";}
	if(id == '20032'){name = "Boni";}
	if(id == '20033'){name = "Hani";}
	if(id == '20034'){name = "Phainos";}
	if(id == '20035'){name = "Bullkano";}
	if(id == '20036'){name = "Tow";}
	if(id == '20037'){name = "Tarok";}
	if(id == '20038'){name = "G'rul";}
	if(id == '20039'){name = "Shu-turul";}
	if(id == '20040'){name = "Bobon";}
	if(id == '20041'){name = "Gund";}
	if(id == '20042'){name = "Croll";}
	if(id == '20043'){name = "Sphatoi";}
	if(id == '20044'){name = "Argo";}
	if(id == '20045'){name = "Bardolf";}
	if(id == '20046'){name = "Zombie";}
	if(id == '20047'){name = "Gaboster";}
	if(id == '20048'){name = "Sharktooth";}
	if(id == '20049'){name = "Noahtle";}
	if(id == '20050'){name = "Succubus";}
    
    /* item */
    if(id == '30001'){name = "Novice Blunt";}
    if(id == '30002'){name = "Novice Dagger";}
    if(id == '30003'){name = "Novice Earing";}
    if(id == '30004'){name = "Novice Heavy Armor";}
    if(id == '30005'){name = "Novice Light Armor";}
    if(id == '30006'){name = "Novice Necklace";}
    if(id == '30007'){name = "Novice Ring";}    
    if(id == '30008'){name = "Novice Sword";}
    if(id == '30009'){name = "Blunt of Forest";}
    if(id == '30010'){name = "Dagger of Forest";}
    if(id == '30011'){name = "Earing of Forest";}
    if(id == '30012'){name = "Heavy Armor of Forest";}
    if(id == '30013'){name = "Light Armor of Forest";}
    if(id == '30014'){name = "Necklace of Forest";}
    if(id == '30015'){name = "Ring of Forest";}    
    if(id == '30016'){name = "Sword of Forest";}
    if(id == '30017'){name = "Forsaken Blunt";}
    if(id == '30018'){name = "Forsaken Dagger";}
    if(id == '30019'){name = "Forsaken Earing";}
    if(id == '30020'){name = "Forsaken Heavy Armor";}
    if(id == '30021'){name = "Forsaken Light Armor";}
    if(id == '30022'){name = "Forsaken Necklace";}
    if(id == '30023'){name = "Forsaken Ring";}    
    if(id == '30024'){name = "Forsaken Sword";}
    if(id == '30025'){name = "Royal Blunt";}
    if(id == '30026'){name = "Royal Dagger";}
    if(id == '30027'){name = "Royal Earing";}
    if(id == '30028'){name = "Royal Heavy Armor";}
    if(id == '30029'){name = "Royal Light Armor";}
    if(id == '30030'){name = "Royal Necklace";}
    if(id == '30031'){name = "Royal Ring";}    
    if(id == '30032'){name = "Royal Sword";}
    
    return name;
}

function cnvs_grade(grade){
	var grade_name = "";
	
	/* grade */
	if(grade == '1'){grade_name = "Legendary.png";}
	if(grade == '2'){grade_name = "Unique.png";}
	if(grade == '3'){grade_name = "Rare.png";}
	if(grade == '4'){grade_name = "Uncommon.png";}
	if(grade == '5'){grade_name = "Common.png";}

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

function cnvs_img(id){
	var img_name = "";
	
	/* servant */
	if(id > '10000' && id < '10010'){ img_name = "servant_warrior_AM.png";}
	if(id > '10009' && id < '10019'){ img_name = "servant_warrior_AF.png";}
	if(id > '10018' && id < '10028'){ img_name = "servant_warrior_KM.png";}
	if(id > '10027' && id < '10037'){ img_name = "servant_warrior_KF.png";}
	if(id > '10036' && id < '10046'){ img_name = "servant_thief_AM.png";}
	if(id > '10045' && id < '10055'){ img_name = "servant_thief_AF.png";}
	if(id > '10054' && id < '10064'){ img_name = "servant_thief_KM.png";}
	if(id > '10063' && id < '10073'){ img_name = "servant_thief_KF.png";}
	if(id > '10072' && id < '10082'){ img_name = "servant_cleric_AM.png";}
	if(id > '10081' && id < '10091'){ img_name = "servant_cleric_AF.png";}
	if(id > '10090' && id < '10100'){ img_name = "servant_cleric_KM.png";}
	if(id > '10099' && id < '10109'){ img_name = "servant_cleric_KF.png";}	
	
	/* monster */
	if(id == '20001'){img_name = "Mst_Panda.png";}
	if(id == '20002'){img_name = "Mst_Panda_Gold.png";}
	if(id == '20003'){img_name = "Mst_Anubis.png";}
	if(id == '20004'){img_name = "Mst_Ranubis.png";}
	if(id == '20005'){img_name = "Mst_Bear_Brown.png";}
	if(id == '20006'){img_name = "Mst_Bear_Black.png";}
	if(id == '20007'){img_name = "Mst_Bear_Red.png";}
	if(id == '20008'){img_name = "Mst_Bear_White.png";}
	if(id == '20009'){img_name = "Mst_Birdman.png";}
	if(id == '20010'){img_name = "Mst_Crowman.png";}
	if(id == '20011'){img_name = "Mst_Card_1.png";}
	if(id == '20012'){img_name = "Mst_Card_2.png";}
	if(id == '20013'){img_name = "Mst_Card_3.png";}
	if(id == '20014'){img_name = "Mst_Card_4.png";}
	if(id == '20015'){img_name = "Mst_Cat_Black.png";}
	if(id == '20016'){img_name = "Mst_Cat_Sham.png";}
	if(id == '20017'){img_name = "Mst_Cat_White.png";}
	if(id == '20018'){img_name = "Mst_Dark_Knight.png";}
	if(id == '20019'){img_name = "Mst_Dark_Magician.png";}
	if(id == '20020'){img_name = "Mst_Death.png";}
	if(id == '20021'){img_name = "Mst_Spectre.png";}
	if(id == '20022'){img_name = "Mst_Egg_1.png";}
	if(id == '20023'){img_name = "Mst_Egg_2.png";}
	if(id == '20024'){img_name = "Mst_Egg_3.png";}
	if(id == '20025'){img_name = "Mst_Fairy_Fire.png";}
	if(id == '20026'){img_name = "Mst_Fairy_Leaf.png";}
	if(id == '20027'){img_name = "Mst_Fairy_Water.png";}
	if(id == '20028'){img_name = "Mst_Gangsi.png";}
	if(id == '20029'){img_name = "Mst_Golem_Moss.png";}
	if(id == '20030'){img_name = "Mst_Golem_Lava.png";}
	if(id == '20031'){img_name = "Mst_Goblin_Blue.png";}
	if(id == '20032'){img_name = "Mst_Goblin_Green.png";}
	if(id == '20033'){img_name = "Mst_Goblin_Red.png";}
	if(id == '20034'){img_name = "Mst_Minotours_Red.png";}
	if(id == '20035'){img_name = "Mst_Minotours_Blue.png";}
	if(id == '20036'){img_name = "Mst_Minotours_Brown.png";}
	if(id == '20037'){img_name = "Mst_Ogre.png";}
	if(id == '20038'){img_name = "Mst_Ogre_Twinhead.png";}
	if(id == '20039'){img_name = "Mst_Ogre_Champion.png";}
	if(id == '20040'){img_name = "Mst_Skeleton_Archer.png";}
	if(id == '20041'){img_name = "Mst_Skeleton_Berserker.png";}
	if(id == '20042'){img_name = "Mst_Skeleton_Giant.png";}
	if(id == '20043'){img_name = "Mst_Skeleton_Soldier.png";}
	if(id == '20044'){img_name = "Mst_Snail.png";}
	if(id == '20045'){img_name = "Mst_Werewolf.png";}
	if(id == '20046'){img_name = "Mst_Zombie.png";}
	if(id == '20047'){img_name = "Mst_Lobster_Soldier.png";}
	if(id == '20048'){img_name = "Mst_Shark_Soldier.png";}
	if(id == '20049'){img_name = "Mst_Turtle_Master.png";}
	if(id == '20050'){img_name = "Mst_Succubus.png";}
    
    /* item */
	if(id == '30001'){img_name = "tier1_blunt.png";}
	if(id == '30002'){img_name = "tier1_dagger.png";}
	if(id == '30003'){img_name = "tier1_earing.png";}
	if(id == '30004'){img_name = "tier1_HeavyA.png";}
	if(id == '30005'){img_name = "tier1_LightA.png";}
	if(id == '30006'){img_name = "tier1_necklace.png";}
	if(id == '30007'){img_name = "tier1_ring.png";}	
	if(id == '30008'){img_name = "tier1_sword.png";}
	if(id == '30009'){img_name = "tier2_blunt.png";}
	if(id == '30010'){img_name = "tier2_dagger.png";}
	if(id == '30011'){img_name = "tier2_earing.png";}
	if(id == '30012'){img_name = "tier2_HeavyA.png";}
	if(id == '30013'){img_name = "tier2_LightA.png";}
	if(id == '30014'){img_name = "tier2_necklace.png";}
	if(id == '30015'){img_name = "tier2_ring.png";}	
	if(id == '30016'){img_name = "tier2_sword.png";}
	if(id == '30017'){img_name = "tier3_blunt.png";}
	if(id == '30018'){img_name = "tier3_dagger.png";}
	if(id == '30019'){img_name = "tier3_earing.png";}
	if(id == '30020'){img_name = "tier3_HeavyA.png";}
	if(id == '30021'){img_name = "tier3_LightA.png";}
	if(id == '30022'){img_name = "tier3_necklace.png";}
	if(id == '30023'){img_name = "tier3_ring.png";}	
	if(id == '30024'){img_name = "tier3_sword.png";}
	if(id == '30025'){img_name = "tier4_blunt.png";}
	if(id == '30026'){img_name = "tier4_dagger.png";}
	if(id == '30027'){img_name = "tier4_earing.png";}
	if(id == '30028'){img_name = "tier4_HeavyA.png";}
	if(id == '30029'){img_name = "tier4_LightA.png";}
	if(id == '30030'){img_name = "tier4_necklace.png";}
	if(id == '30031'){img_name = "tier4_ring.png";}	
	if(id == '30032'){img_name = "tier4_sword.png";}
	
	return img_name;
}

function cnvs_cd(grade){
	var cd_name = "";
	
	/* grade */
	if(grade == '1'){cd_name = "card1.png";}
	if(grade == '2'){cd_name = "card2.png";}
	if(grade == '3'){cd_name = "card3.png";}
	if(grade == '4'){cd_name = "card4.png";}
	if(grade == '5'){cd_name = "card5.png";}

	return cd_name;
}

function cnvs_tier(tier){
	var img_name = "";
	
	if(tier == '1'){img_name = "icon_tier_1.png";}
	if(tier == '2'){img_name = "icon_tier_2.png";}
	if(tier == '3'){img_name = "icon_tier_3.png";}
	if(tier == '4'){img_name = "icon_tier_4.png";}
	
	return img_name;
}

function cnvs_type(type){	
	var img_name = "";
	
	if(type == '1'){img_name = "icon_stat_atk.png";}
	if(type == '2'){img_name = "icon_stat_def.png";}
	if(type == '3'){img_name = "icon_stat_str.png";}
	if(type == '4'){img_name = "icon_stat_dex.png";}
	if(type == '5'){img_name = "icon_stat_int.png";}
	
	return img_name;
}

