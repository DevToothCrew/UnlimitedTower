const scatterEOS = window.ScatterEOS;
const scatterJS = window.ScatterJS;

//document.write('<script src="assets/js/conversion.js"></script>');

function addJavascript(jsname) {
	var th = document.getElementsByTagName('head')[0];
	var s = document.createElement('script');
	s.setAttribute('type','text/javascript');
	s.setAttribute('src',jsname);
	th.appendChild(s);
}

addJavascript('assets/js/conversion.js?ver=6');

function scatter_login(){

		    		

		$(".sec3-btn1").css({"display":"none"});  //Please login first
		$(".sec3-btn2").css({"display":"block"}); //Already Registered
		$(".sec3-btn").css({"display":"none"});   //Pre-Register
		
		$(".sec4-btn1").css({"display":"block"}); //Play Gacha
		$(".sec4-btn2").css({"display":"none"});  //Please login first
		$(".sec4-btn").css({"display":"none"});   //Register first
		
		$(".section4-3").css({"display" :"block"})
		    		


}

function register(){
	scatterJS.plugins( new scatterEOS() );

	const network = {
			blockchain:'eos',
			protocol:'https',
			host:'jungle2.cryptolions.io',
			port:443,
			chainId:'e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473'
		}
	scatterJS.scatter.connect('My-App').then(connected => {
		const scatter = scatterJS.scatter;
		
		if(!connected) return false;
		
		const requiredFields = { accounts:[network] };
	
		scatter.getIdentity(requiredFields).then(() => {
			const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
			const eosOptions = { expireInSeconds:60 };
			const eos = scatter.eos(network, Eos, eosOptions);
			const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };

			var login_seed;
			var login_num;
			$.post("https://dcugl.com:5000/seed", {})
			.done(function(data){
				login_seed = data.seed;
				login_num = data.num;
			});

			$.post("https://dcugl.com:5000/prelogin", { user:account.name })
		    .done(function(data){
		    	console.log(data);	    	
		    	
		    	if(data.signup != true){
		    		$(".sec3-btn1").css({"display":"none"}); //Please login first
		    		$(".sec3-btn2").css({"display":"none"}); //Already Registered
		    		$(".sec3-btn").css({"display":"block"}); //Pre-Register
		    		
		    		$(".sec4-btn1").css({"display":"none"}); //Play Gacha
		    		$(".sec4-btn2").css({"display":"none"}); //Please login first
		    		$(".sec4-btn").css({"display":"block"}); //Register first
		    		
		    		console.log('tiger');
		    		
		    		eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'presignup:'+login_num+':'+login_seed, transactionOptions).then(trx => {
						console.log(`Transaction ID: ${trx.transaction_id}`);
					}).catch(error => {
						$(".cbtn").css({"display" :"block"});
						console.log('error');
					});
		    		
		    		$.post("https://dcugl.com:5000/presignup", { user:account.name })
				    .done(function(data){
				    	console.log(data);
			    		$(".sec3-btn1").css({"display":"none"});  //Please login first
			    		$(".sec3-btn2").css({"display":"block"}); //Already Registered
			    		$(".sec3-btn").css({"display":"none"});   //Pre-Register
			    		
			    		$(".sec4-btn1").css({"display":"block"}); //Play Gacha
			    		$(".sec4-btn2").css({"display":"none"});  //Please login first
			    		$(".sec4-btn").css({"display":"none"});   //Register first
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

							$('.slide-wrap').slick('slickAdd',html)
			    			
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
			    			
			    			$('.slide-wrap').slick('slickAdd',html)

			    		}
			    		if(data.item_list.length > 0){
				    		console.log('itemOK');
				    		for(var i = 0;data.item_list.length > i;i++){
				    			var html = "";

				    			html = "<div class='slick slick-slide'>";
								html = html + "<img class='scard-bg legendary' src='assets/img/"+cnvs_cd(data.item_list[i].grade)+"' alt=''>";
								html = html + "<img class='stier' src='assets/img/icon/tier/"+cnvs_tier(data.item_list[i].tier)+"' alt=''>";
								html = html + "<h2>"+cnvs_name(result.data.id)+"</h2>";
								html = html + "<img class='smonster' src='assets/img/characters/item/"+cnvs_img(data.item_list[i].id)+"' alt=''>";
								html = html + "<div class='ability'>";							
								html = html + "<div class='sabil'><img src='assets/img/"+cnvs_type(data.item_list[i].type)+"' alt=''><p>"+cnvs_rank(data.item_list[i].main_status)+"</p></div>";
								html = html + "</div>";
								html = html + "</div>";
								
								$('.slide-wrap').slick('slickAdd',html)
				    		}
				    	}
			    		console.log('servantOK');
			    	}
		    	}
		    });
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
	if(this.scatter.identity){
		this.scatter.forgetIdentity();
		
		$(".slide-wrap").empty();
		
		$(".logout").slideToggle();
		$(".login").css({"display":"block"});
		$(".name").css({"display":"none"});
		
		$(".sec3-btn1").css({"display":"block"});  //Please login first
		$(".sec3-btn2").css({"display":"none"}); //Already Registered
		$(".sec3-btn").css({"display":"none"});   //Pre-Register
		
		$(".sec4-btn1").css({"display":"none"}); //Play Gacha
		$(".sec4-btn2").css({"display":"block"});  //Please login first
		$(".sec4-btn").css({"display":"none"});   //Register first
	}
}

function gacha(){
	//$(".sec4-btn1").css({"display":"none"}); //Play Gacha
	$('.sec4-btn1').bind('click', false); //Play Gacha
			    			
	
	html = "<div class='card-bg'><img src='assets/img/legendary.png' alt=''></div>";
	html = html + "<div class='card'>";
	html = html + "<h3 class='cname'>Emperor Panda</h3>";
	html = html + "<img class='bmonster' src='assets/img/characters/monster/Mst_Panda_Gold.png' alt=''>";
	html = html + "<div class='value'>";
	html = html + "<div class='val'><img src='assets/img/icon_stat_str.png' alt=''><p>S+</p></div>";
	html = html + "<div class='val'><img src='assets/img/icon_stat_dex.png' alt=''><p>S+</p></div>";
	html = html + "<div class='val'><img src='assets/img/icon_stat_int.png' alt=''><p>S</p></div>";
	html = html + "</div>";
	html = html + "</div>";	
	html = html + "<div class='sec4-2btn'>";
	html = html + "<a href='javascript:gacha_ok()'>OK</a>";
	html = html + "</div>";
	

	$(".card-rota").append(html);
			    		
	$(".section4").css({"display" :"none"});
    $(".sec4-1").delay(1000).addClass("show4-1");
    $(".section4-1").css({"height" :"1084px","opacity" :"1"});
    $(".section4-3").css({"display" :"block"});
}

function gacha_ok(){
	$('.sec4-btn1').unbind('click', false); //Play Gacha
	$(".sec4-1").removeClass("show4-1");
	$(".section4-1").css({"height" :"1px","opacity" :"0"});
	$(".section4").css({"display" :"block"});
	$(".card-rota").empty()
	//$(".card-rota").remove();
}
