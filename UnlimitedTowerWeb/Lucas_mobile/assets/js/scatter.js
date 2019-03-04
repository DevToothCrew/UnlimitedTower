const scatterEOS = window.ScatterEOS;
const scatterJS = window.ScatterJS;

ScatterJS.plugins( new ScatterEOS() );

let scatter,requiredFields;

const network = {
    blockchain:'eos',
    protocol:'https',
    host:'nodes.get-scatter.com',
    port:443,
    chainId:'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906'
}

function scatter_connect(){
	scatterJS.scatter.connect('My-App').then(connected => {
		scatter = scatterJS.scatter;
		if(!connected) return false;
		requiredFields = { accounts:[network] };
			
		console.log('connect');
		console.log(scatter);
		
		scatter_login();
	});
}

var contract = "untowermain1";
var signup = "";

var server_time = 0;
var user_time = 0;
var user_total_day = 0;
var user_name = "";
var user_check_yn = "y";

function addJavascript(jsname) {
	var th = document.getElementsByTagName('head')[0];
	var s = document.createElement('script');
	s.setAttribute('type','text/javascript');
	s.setAttribute('src',jsname);
	th.appendChild(s);
}

function scatter_login(){	
	var main_form = document.main;
	
	console.log('login');
	console.log(scatter);
	
	scatter.getIdentity(requiredFields).then(() => {
		const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
		const eosOptions = { expireInSeconds:60 };
		const eos = scatter.eos(network, Eos, eosOptions);
		const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };
	
		main_form.userId.value = account.name;
			
		$(".login").css({"display":"none"});
		$(".name").css({"display":"block"});			
		
		var html = "<a href='javascript:name_click();'>"+account.name+"</a>";
		$(".name").empty();
		$(".name").append(html);
					
		$.post("https://dcugl.com/prelogin", { user:account.name })
	    .done(function(data){
	    	const result_data = data;
	    	signup = data.signup;
	    	login_yn = "y";
	    	console.log(result_data);
	    	
	    	if(result_data.signup != true){
	    		$(".sec3-btn1").css({"display":"none"}); //Please login first
	    		$(".sec3-btn2").css({"display":"none"}); //Already Registered
	    		$(".sec3-btn").css({"display":"block"}); //Pre-Register
	
	    		$(".sec4-btn1").css({"display":"none"}); //Play Gacha
	    		$(".sec4-btn2").css({"display":"none"}); //Please login first
	    		$(".sec4-btn").css({"display":"block"}); //Register first
	
	    		console.log('tiger');
	    	}else{
	    		$(".sec3-btn1").css({"display":"none"});  //Please login first
	    		$(".sec3-btn2").css({"display":"block"}); //Already Registered
	    		$(".sec3-btn").css({"display":"none"});   //Pre-Register
	
	    		$(".sec4-btn1").css({"display":"block"}); //Play Gacha
	    		$(".sec4-btn2").css({"display":"none"});  //Please login first
	    		$(".sec4-btn").css({"display":"none"});   //Register first
	
	    		$(".section4-3").css({"display" :"block"});
	
	    		if(result_data.monster_list.length > 0){
	    			for(var i = 0;result_data.monster_list.length > i;i++){
	    				var html = "";
	
	    				html = "<div class='slick'>";
	    				html = html + "<img class='scard-bg' src='assets/img/"+cnvs_cd(result_data.monster_list[i].grade)+"' alt=''>";
	    				html = html + "<h2>"+cnvs_name(result_data.monster_list[i].id)+"</h2>";
	    				html = html + "<img class='smonster' src='assets/img/characters/monster/"+cnvs_img(result_data.monster_list[i].id)+"' alt=''>";
	    				html = html + "<div class='ability'>";
	    				html = html + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(result_data.monster_list[i].status.basic_str)+"</p></div>";
	    				html = html + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(result_data.monster_list[i].status.basic_dex)+"</p></div>";
	    				html = html + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(result_data.monster_list[i].status.basic_int)+"</p></div>";
	    				html = html + "</div>";
	    				html = html + "</div>";
	
	    				$('.slide-wrap').slick('slickAdd',html);
	    			}
	    			console.log('monsterOK');
	    		}
	    		if(result_data.servant_list.length > 0){
	    			for(var i = 0;result_data.servant_list.length > i;i++){
	    				var html = "";
	
	    				html = "<div class='slick'>";
	    				html = html + "<img class='scard-bg' src='assets/img/card5.png' alt=''>";
	    				html = html + "<h2>"+cnvs_name(result_data.servant_list[i].id)+"</h2>";
	    				html = html + "<img class='smonster' src='assets/img/characters/servant/"+cnvs_img(result_data.servant_list[i].id)+"' alt=''>";
	    				html = html + "<div class='ability'>";
	    				html = html + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(result_data.servant_list[i].status.basic_str)+"</p></div>";
	    				html = html + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(result_data.servant_list[i].status.basic_dex)+"</p></div>";
	    				html = html + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(result_data.servant_list[i].status.basic_int)+"</p></div>";
	    				html = html + "</div>";
	    				html = html + "</div>";
	
	    				$('.slide-wrap').slick('slickAdd',html);
	    			}
	    			console.log('servantOK');
	    		}
	    		if(result_data.item_list.length > 0){
	    			console.log('itemOK');
	    			for(var i = 0;result_data.item_list.length > i;i++){
	    				var html = "";
	
	    				html = "<div class='slick slick-slide'>";
	    				html = html + "<img class='scard-bg legendary' src='assets/img/"+cnvs_cd(result_data.item_list[i].grade)+"' alt=''>";
	    				html = html + "<img class='stier' src='assets/img/icon/tier/"+cnvs_tier(result_data.item_list[i].tier)+"' alt=''>";
	    				html = html + "<h2>"+cnvs_name(result_data.item_list[i].id)+"</h2>";
	    				html = html + "<img class='smonster' src='assets/img/characters/item/"+cnvs_img(result_data.item_list[i].id)+"' alt=''>";
	    				html = html + "<div class='ability'>";
	    				html = html + "<div class='sabil'><img src='assets/img/"+cnvs_type(result_data.item_list[i].type)+"' alt=''><p>"+cnvs_rank(result_data.item_list[i].main_status)+"</p></div>";
	    				html = html + "</div>";
	    				html = html + "</div>";
	
	    				$('.slide-wrap').slick('slickAdd',html);
	    			}
	    		}
	    	}
	    });
	});
}

function register(ref_id){
	var ref = "";
	
	scatter.getIdentity(requiredFields).then(() => {
		const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
		const eosOptions = { expireInSeconds:60 };
		const eos = scatter.eos(network, Eos, eosOptions);
		const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };

		var login_seed;
		var login_num;
		
		if(ref_id != ""){
			ref = "refpresignup:"+ref_id+":";
			//ref = "refpresignup:unlimitedmas:";
		}else{
			ref = "presignup:";
		}
		
		$.post("https://dcugl.com/seed", {})
		.done(function(data){
			login_seed = data.seed;
			login_num = data.num;
		});

		$.post("https://dcugl.com/prelogin", { user:account.name })
	    .done(function(data){
	    	signup = data.signup;
	    	console.log(data);
	    
			if(data.signup != true){
	    		$(".sec3-btn1").css({"display":"none"}); //Please login first
	    		$(".sec3-btn2").css({"display":"none"}); //Already Registered
	    		$(".sec3-btn").css({"display":"block"}); //Pre-Register
	    		
	    		$(".sec4-btn1").css({"display":"none"}); //Play Gacha
	    		$(".sec4-btn2").css({"display":"none"}); //Please login first
	    		$(".sec4-btn").css({"display":"block"}); //Register first
	    		
	    		console.log('tiger');
	    		
	    		eos.transfer(account.name, contract , '1.0000 EOS', ref+login_num+':'+login_seed, transactionOptions).then(trx => {
					console.log(`Transaction ID: ${trx.transaction_id}`);
				}).catch(error => {
					var er = JSON.parse(error);
					console.log(er.error.details[0].message);
					
					if(er.code == "500"){alert(er.error.details[0].message); return;}
			    });
    		
	    		$.post("https://dcugl.com/mainpresignup", { user:account.name })
			    .done(function(data){
			    	console.log(data);
			    	
		    		if(data == 'ERR:Time Out'){
			    		console.log('signup TimeOut');				    		
			    		return;
		    		}
		    		
		    		$(".sec3-btn1").css({"display":"none"});  //Please login first
		    		$(".sec3-btn2").css({"display":"block"}); //Already Registered
		    		$(".sec3-btn").css({"display":"none"});   //Pre-Register
		    		
		    		$(".sec4-btn1").css({"display":"block"}); //Play Gacha
		    		$(".sec4-btn2").css({"display":"none"});  //Please login first
		    		$(".sec4-btn").css({"display":"none"});   //Register first

		    		$(".section4-3").css({"display" :"block"});
		    		
		    		signup = true;
		    		
		    		if(data.result == "mon"){
		    			var html = "";
		    			
		    			console.log(cnvs_name(data.id));
		    			
		    			html = "<div class='slick'>";
		    			html = html + "<img class='scard-bg' src='assets/img/"+cnvs_cd(data.grade)+"' alt=''>";
		    			html = html + "<h2>"+cnvs_name(data.id)+"</h2>";
		    			html = html + "<img class='smonster' src='assets/img/characters/monster/"+cnvs_img(data.id)+"' alt=''>";
		    			html = html + "<div class='ability'>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(data.b_str)+"</p></div>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(data.b_dex)+"</p></div>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(data.b_int)+"</p></div>";
		    			html = html + "</div>";
		    			html = html + "</div>";

						$('.slide-wrap').slick('slickAdd',html);
		    		}
			    });
	    		
	    	}else{
	    		$(".sec3-btn1").css({"display":"none"});  //Please login first
	    		$(".sec3-btn2").css({"display":"block"}); //Already Registered
	    		$(".sec3-btn").css({"display":"none"});   //Pre-Register
	    		
	    		$(".sec4-btn1").css({"display":"block"}); //Play Gacha
	    		$(".sec4-btn2").css({"display":"none"});  //Please login first
	    		$(".sec4-btn").css({"display":"none"});   //Register first
	    		
	    		$(".section4-3").css({"display" :"block"})
	    		
		    	if(data.monster_list.length > 0){
		    		for(var i = 0;data.monster_list.length > i;i++){
		    			var html = "";

		    			html = "<div class='slick'>";
		    			html = html + "<img class='scard-bg' src='assets/img/"+cnvs_cd(data.monster_list[i].grade)+"' alt=''>";
		    			html = html + "<h2>"+cnvs_name(data.monster_list[i].id)+"</h2>";
		    			html = html + "<img class='smonster' src='assets/img/characters/monster/"+cnvs_img(data.monster_list[i].id)+"' alt=''>";
		    			html = html + "<div class='ability'>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(data.monster_list[i].status.basic_str)+"</p></div>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(data.monster_list[i].status.basic_dex)+"</p></div>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(data.monster_list[i].status.basic_int)+"</p></div>";
		    			html = html + "</div>";
		    			html = html + "</div>";

						$('.slide-wrap').slick('slickAdd',html);			    			
		    		}
		    		console.log('monsterOK');
		    	}
		    	if(data.servant_list.length > 0){
		    		for(var i = 0;data.servant_list.length > i;i++){
		    			var html = "";

		    			html = "<div class='slick'>";
		    			html = html + "<img class='scard-bg' src='assets/img/card5.png' alt=''>";
		    			html = html + "<h2>"+cnvs_name(data.servant_list[i].id)+"</h2>";
		    			html = html + "<img class='smonster' src='assets/img/characters/servant/"+cnvs_img(data.servant_list[i].id)+"' alt=''>";
		    			html = html + "<div class='ability'>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_str)+"</p></div>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_dex)+"</p></div>";
		    			html = html + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_int)+"</p></div>";
		    			html = html + "</div>";
		    			html = html + "</div>";
		    			
		    			$('.slide-wrap').slick('slickAdd',html);

		    		}
		    		if(data.item_list.length > 0){
			    		console.log('itemOK');
			    		for(var i = 0;data.item_list.length > i;i++){
			    			var html = "";

			    			html = "<div class='slick slick-slide'>";
							html = html + "<img class='scard-bg legendary' src='assets/img/"+cnvs_cd(data.item_list[i].grade)+"' alt=''>";
							html = html + "<img class='stier' src='assets/img/icon/tier/"+cnvs_tier(data.item_list[i].tier)+"' alt=''>";
							html = html + "<h2>"+cnvs_name(data.item_list[i].id)+"</h2>";
							html = html + "<img class='smonster' src='assets/img/characters/item/"+cnvs_img(data.item_list[i].id)+"' alt=''>";
							html = html + "<div class='ability'>";							
							html = html + "<div class='sabil'><img src='assets/img/"+cnvs_type(data.item_list[i].type)+"' alt=''><p>"+cnvs_rank(data.item_list[i].main_status)+"</p></div>";
							html = html + "</div>";
							html = html + "</div>";
							
							$('.slide-wrap').slick('slickAdd',html);
			    		}
			    	}
		    		console.log('servantOK');
		    	}
	    	}
		}).catch(error => {
			if(error.code != '402'){
				console.log(error);
				if(error.code == '423'){
					console.log('scatter 잠김 !!');
					console.log(error);					
				}else{
					document.main.submit();					
				}
			}else{
				console.log('login 거부 !!');
				console.log(error);
			}
		});
	});
}

function scatter_logout(){	
	const scatter = scatterJS.scatter;
	var slideIndex=10000;
	
	scatter.logout();
	
	$('.slide-wrap').slick('slickRemove',slideIndex - 10000);
	 if (slideIndex !== 0){
	   slideIndex--;
	 }
	
	$(".logout").slideToggle();
	$(".login").css({"display":"block"});
	$(".name").css({"display":"none"});
	
	$(".sec3-btn1").css({"display":"block"});  //Please login first
	$(".sec3-btn2").css({"display":"none"}); //Already Registered
	$(".sec3-btn").css({"display":"none"});   //Pre-Register
	
	$(".sec4-btn1").css({"display":"none"}); //Play Gacha
	$(".sec4-btn2").css({"display":"block"});  //Please login first
	$(".sec4-btn").css({"display":"none"});   //Register first
	
	$(".section4-3").css({"display" :"none"});

	$(".date").removeClass("active");
	$(".date").removeClass("check");
	
	user_time = 0;
	user_total_day = 0;
	user_name = "";
	user_check_yn = "y";
	
	signup = "";
	login_yn = "n";
}

var html2 = ""; 

function gacha(){
	scatter.getIdentity(requiredFields).then(() => {
		const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
		const eosOptions = { expireInSeconds:60 };
		const eos = scatter.eos(network, Eos, eosOptions);
		const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };
		
		$.post("https://dcugl.com/seed", {})
	    .done(function(data){
	    		console.log('data_num -->' + data.num);
	    		console.log('data_seed -->' + data.seed);
	    		console.log('gacha:'+data.num+':'+data.seed);
	    		eos.transfer(account.name, contract , '1.0000 EOS', 'gacha:'+data.num+':'+data.seed, transactionOptions).then(trx => {
				console.log(`Transaction ID: ${trx.transaction_id}`);
				
				$(".section4").css({"display" :"none"});			    		
	    		$(".section4-1").css({"height" :"1084px","opacity" :"1"});	                    
                $(".section4-3").css({"display" :"block"});
                
			}).catch(error => {
				var er = JSON.parse(error);
				console.log(er.error.details[0].message);
				
				if(er.code == "500"){alert(er.error.details[0].message); return;}

	    		$(".sec4-1").removeClass("show4-1");
	    		$(".section4-1").css({"height" :"1px","opacity" :"0"});
	    		$(".section4").css({"display" :"block"});
			});

	    	$.post("https://dcugl.com/mainpregacha", { user:account.name })
		    .done(function(result,result_type){
		    	const result_data = result;
		    	
		    	console.log('gacha!!:');
		    	
		    	console.log(result_data);
				if(result_data != 'Fail to get gacha data'){
		    		if(result_data == 'ERR:Time Out'){
			    		console.log('TimeOut');
			    		$('.sec4-btn1').unbind('click', false); //Play Gacha
			    		$(".sec4-1").removeClass("show4-1");
			    		$(".section4-1").css({"height" :"1px","opacity" :"0"});
			    		$(".section4").css({"display" :"block"});
			    		
			    		return;
		    		}			    		
		    		if(result_data.result == 'mon'){
		    			var html = "";

		    			console.log(cnvs_name(result_data.id));			    			
		    			
		    			html2 = "<div class='slick'>";
		    			html2 = html2 + "<img class='scard-bg' src='assets/img/"+cnvs_cd(result_data.grade)+"' alt=''>";
		    			html2 = html2 + "<h2>"+cnvs_name(result_data.id)+"</h2>";
		    			html2 = html2 + "<img class='smonster' src='assets/img/characters/monster/"+cnvs_img(result_data.id)+"' alt=''>";
		    			html2 = html2 + "<div class='ability'>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(result_data.b_str)+"</p></div>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(result_data.b_dex)+"</p></div>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(result_data.b_int)+"</p></div>";
		    			html2 = html2 + "</div>";
		    			html2 = html2 + "</div>";

		    			html = "<div class='card-bg'><img src='assets/img/"+cnvs_grade(result_data.grade)+"' alt=''></div>";
		    			html = html + "<div class='card'>";
		    			html = html + "<h3 class='cname'>"+cnvs_name(result_data.id)+"</h3>";
		    			html = html + "<img class='bmonster' src='assets/img/characters/monster/"+cnvs_img(result_data.id)+"' alt=''>";
		    			html = html + "<div class='value'>";
		    			html = html + "<div class='val'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(result_data.b_str)+"</p></div>";
		    			html = html + "<div class='val'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(result_data.b_dex)+"</p></div>";
		    			html = html + "<div class='val'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(result_data.b_int)+"</p></div>";
		    			html = html + "</div>";
		    			html = html + "</div>";			    			
		    			html = html + "<div class='sec4-2btn'>";
		    			html = html + "<a href='javascript:gacha_ok()'>OK</a>";
		    			html = html + "</div>";
		    			
		    			$(".card-rota").append(html);
		    		}
		    		if(result_data.result == 'ser'){
		    			var html = "";

		    			console.log(cnvs_name(result_data.id));
		    			
		    			html2 = "<div class='slick'>";
		    			html2 = html2 + "<img class='scard-bg' src='assets/img/card5.png' alt=''>";
		    			html2 = html2 + "<h2>"+cnvs_name(result_data.id)+"</h2>";
		    			html2 = html2 + "<img class='smonster' src='assets/img/characters/servant/"+cnvs_img(result_data.id)+"' alt=''>";
		    			html2 = html2 + "<div class='ability'>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(result_data.b_str)+"</p></div>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(result_data.b_dex)+"</p></div>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(result_data.b_int)+"</p></div>";
		    			html2 = html2 + "</div>";
		    			html2 = html2 + "</div>";

		    			html = "<div class='card-bg'><img src='assets/img/Common.png' alt=''></div>";
		    			html = html + "<div class='card'>";
		    			html = html + "<h3 class='cname'>"+cnvs_name(result_data.id)+"</h3>";
		    			html = html + "<img class='bservant' src='assets/img/characters/servant/"+cnvs_img(result_data.id)+"' alt=''>";
		    			html = html + "<div class='value'>";
		    			html = html + "<div class='val'><img src='assets/img/icon_stat_str.png' alt=''><p>"+cnvs_rank(result_data.b_str)+"</p></div>";
		    			html = html + "<div class='val'><img src='assets/img/icon_stat_dex.png' alt=''><p>"+cnvs_rank(result_data.b_dex)+"</p></div>";
		    			html = html + "<div class='val'><img src='assets/img/icon_stat_int.png' alt=''><p>"+cnvs_rank(result_data.b_int)+"</p></div>";
		    			html = html + "</div>";
		    			html = html + "</div>";
		    			html = html + "<div class='sec4-2btn'>";
		    			html = html + "<a href='javascript:gacha_ok()'>OK</a>";
		    			html = html + "</div>";

		    			$(".card-rota").append(html);
		    		}
		    		if(result_data.result == 'itm'){
		    			var html = "";

		    			console.log(cnvs_name(result_data.id));
		    			
		    			html2 = "<div class='slick slick-slide'>";
		    			html2 = html2 + "<img class='scard-bg legendary' src='assets/img/"+cnvs_cd(result_data.grade)+"' alt=''>";
		    			html2 = html2 + "<img class='stier' src='assets/img/icon/tier/"+cnvs_tier(result_data.tier)+"' alt=''>";
		    			html2 = html2 + "<h2>"+cnvs_name(result_data.id)+"</h2>";
		    			html2 = html2 + "<img class='smonster' src='assets/img/characters/item/"+cnvs_img(result_data.id)+"' alt=''>";
		    			html2 = html2 + "<div class='ability'>";
		    			html2 = html2 + "<div class='sabil'><img src='assets/img/"+cnvs_type(result_data.type)+"' alt=''><p>"+cnvs_rank(result_data.main_status)+"</p></div>";
		    			html2 = html2 + "</div>";
		    			html2 = html2 + "</div>";
		    			
		    			html = "<div class='card-bg'><img src='assets/img/"+cnvs_grade(result_data.grade)+"' alt=''></div>";
		    			html = html + "<div class='card item'>";
		    			html = html + "<img class='tier' src='assets/img/icon/tier/"+cnvs_tier(result_data.tier)+"' alt=''>";
		    			html = html + "<h3 class='cname'>"+cnvs_name(result_data.id)+"</h3>";
		    			html = html + "<img class='bitem' src='assets/img/characters/item/"+cnvs_img(result_data.id)+"' alt=''>";
		    			html = html + "<div class='value'>";
		    			html = html + "<div class='val'><img src='assets/img/"+cnvs_type(result_data.type)+"' alt=''><p>"+cnvs_rank(result_data.main_status)+"</p></div>";
		    			html = html + "</div>";
		    			html = html + "</div>";			    			
		    			html = html + "<div class='sec4-2btn'>";
		    			html = html + "<a href='javascript:gacha_ok()'>OK</a>";
		    			html = html + "</div>";

		    			$(".card-rota").append(html);
		    		}
		    		
		    		$(".sec4-1").addClass("show4-1");
		    	}else{
		    		console.log('gacha 거부!');

		    		$('.sec4-btn1').unbind('click', false); //Play Gacha
		    		$(".sec4-1").removeClass("show4-1");
		    		$(".section4-1").css({"height" :"1px","opacity" :"0"});
		    		$(".section4").css({"display" :"block"});
		    	}
		    });
	    });
	});
}

function gacha_ok(result){	
	$(".sec4-1").removeClass("show4-1");
	$(".section4-1").css({"height" :"1px","opacity" :"0"});
	$(".section4").css({"display" :"block"});
	$(".card-rota").empty();
	
	$('.slide-wrap').slick('slickAdd',html2);
	html2 = "";
}

function daily_open(){
	var d = new Date();
	
	var s_time1 = ( d.getTime() + 32400 ) / 86400;
	var s_time2 = s_time1.toString();
	server_time = s_time2.substr(0,5); 
	console.log(server_time);

	scatter.getIdentity(requiredFields).then(() => {
		const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
		const eosOptions = { expireInSeconds:60 };
		const eos = scatter.eos(network, Eos, eosOptions);
		const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };

		eos.getTableRows({
	       code : contract,
	       scope : contract,
	       table : 'tdaily',
	       lower_bound : account.name,
	       limit : 1,
	       json : true
	    }).then(result=>{		    	
	    	if(result.rows.length != 0){
	    		if(result.rows[0].user == account.name ){
	    			user_time = result.rows[0].check_time;
		    		user_total_day = result.rows[0].total_day;
		    		user_name = result.rows[0].user;
		    		
		    		for(var i = 0;i<result.rows[0].total_day;i++){
		    			$("#day"+(i+1)).addClass("check");
		    		}
		    	}else{
		    		console.log('비정상접근');
		    		$("#day1").addClass("active");
		    		user_check_yn = 'n';
		    		return;
		    	}
	    		
	    		if(user_time == server_time){
	    			$("#day"+(result.rows[0].total_day)).addClass("active");
	    		}else{
	    			$("#day"+(result.rows[0].total_day + 1)).addClass("active");
	    		}
	    	}else{
	    		// 첫 출석체크
	    		$("#day1").addClass("active");
	    	}
	    });
	}).catch(error => {
		console.log(error);
	});

	$("#popup").css({"display":"block"});
}

function daily_check(){
	if(login_yn != 'y'){
		alert("Login first");
		return;
	}
	if(signup != true){
		alert("Register first");
		return;		
	}

	scatter.getIdentity(requiredFields).then(() => {
		const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
		const eosOptions = { expireInSeconds:60 };
		const eos = scatter.eos(network, Eos, eosOptions);
		const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };
		
		var daily_seed = "";
		
		if(user_time != server_time || user_check_yn == 'n'){
			$.post("https://dcugl.com/seed", {})
			.done(function(data){
				console.log(data.seed);
				daily_seed = data.num+':'+data.seed;
				console.log(daily_seed);
				
				eos.transaction({
					actions: [
						{
							account: 'untowermain1',
							name: 'dailycheck',
							authorization: [{
								actor: account.name,
								permission: 'active'
							}],
							
							data: {
							  _user : account.name,
							  _seed : daily_seed
							}
						}
					]
				}).then(function(data){
					if(document.main.country.value == 'en'){
						alert('Attendance check is complete!');
					}else if(document.main.country.value == 'ko'){
						alert('출석체크 완료!');
					}else if(document.main.country.value == 'ch'){
						alert('已签到！');
					}
					$("#day"+(user_total_day+1)).addClass("check");
					user_check_yn = 'y';
					user_time = server_time;
				});					
			});
			
		}else{
			if(document.main.country.value == 'en'){
				alert('Attendance check has already been completed!');
			}else if(document.main.country.value == 'ko'){
				alert('이미 출석체크가 완료되었습니다!');
			}else if(document.main.country.value == 'ch'){
				alert('今天已签到');
			}
			
			return;
		}
	}).catch(error => {
		console.log(error);
	});
}