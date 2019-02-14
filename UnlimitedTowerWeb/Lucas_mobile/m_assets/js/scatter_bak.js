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

addJavascript('assets/js/conversion.js?ver=3');

function scatter_login(){
	$(".cbtn").css({"display" :"none"});
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
		
		var main_form = document.main;
		
		if(!connected) return false;
		
		const requiredFields = { accounts:[network] };
	
		scatter.getIdentity(requiredFields).then(() => {
			const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
			const eosOptions = { expireInSeconds:60 };
			const eos = scatter.eos(network, Eos, eosOptions);
			const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };
			
			main_form.userId.value = account.name;
			
			//alert(account.name);
			
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
		    	if(data.signup == null){
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
				    	
				    	$(".section3").css({"display" :"none"});
			            $(".section3-1").css({"display" :"block"});
			            $(".section3-3").css({"display" :"block"});
				    });
		    	}else{
		    		$.post("https://dcugl.com:5000/prelogin", { user:account.name })
				    .done(function(data){
				    	console.log('else : ' + data);
				    	
				    	if(data.item_list.length > 0){
				    		console.log('itemOK');
				    		for(var i = 0;data.item_list.length > i;i++){
				    			var html = "";
				    			
				    			html = "<div class='hov gg'>";
				    			html = html + "<img class='cardbg' src='assets/img/"+cnvs_cd(data.item_list[i].grade)+"?ver=1' alt=''>";
				    			html = html + "<div class='item'>";
				    			html = html + "<div class='tier'><img src='assets/img/icon/tier/"+cnvs_tier(data.item_list[i].tier)+"' alt=''></div>";
				    			html = html + "<div class='tr'><img src='assets/img/icon/item_type/"+cnvs_type(data.item_list[i].type)+"' alt=''></div>";
								html = html + "<img class='spanda' src='assets/img/characters/item/"+cnvs_img(data.item_list[i].id)+"' alt=''>";
								html = html + "<p  class='ct1'>"+cnvs_grade(data.item_list[i].grade)+"</p>";
								html = html + "<h1>"+cnvs_name(data.item_list[i].id)+"</h1>";
								html = html + "<div class='s-rank'>";
								html = html + "<div class='s-rk'><img src='assets/img/s-value1.png' alt=''><p>"+cnvs_rank(data.item_list[i].main_status)+"</p></div>";
								html = html + "</div>";
								html = html + "</div>";
								html = html + "</div>";
				    			
				    			$(".card-cont").append(html);
				    		}
				    	}
				    	if(data.monster_list.length > 0){
				    		for(var i = 0;data.monster_list.length > i;i++){
				    			var html = "";
				    			
				    			html = "<div class='hov gg'>";
				    			html = html + "<img class='cardbg' src='assets/img/"+cnvs_cd(data.monster_list[i].grade)+"?ver=1' alt=''>";
				    			html = html + "<div class='hov-b'>";
								html = html + "<img class='spanda' src='assets/img/characters/monster/"+cnvs_img(data.monster_list[i].id)+"' alt=''>";
								html = html + "<p class='ct1'>"+cnvs_grade(data.monster_list[i].grade)+"</p>";
								html = html + "<h1>"+cnvs_name(data.monster_list[i].id)+"</h1>";
								html = html + "<div class='s-rank'>";
								html = html + "<div class='s-rk'><img src='assets/img/s-value1.png' alt=''><p>"+cnvs_rank(data.monster_list[i].status.basic_str)+"</p></div> ";
								html = html + "<div class='s-rk'><img src='assets/img/s-value2.png' alt=''><p>"+cnvs_rank(data.monster_list[i].status.basic_dex)+"</p></div> ";
								html = html + "<div class='s-rk'><img src='assets/img/s-value3.png' alt=''><p>"+cnvs_rank(data.monster_list[i].status.basic_int)+"</p></div> ";									 
								html = html + "</div>";
								html = html + "</div>";
								html = html + "</div>";
				    			
				    			$(".card-cont").append(html);
				    		}
				    		console.log('monsterOK');
				    	}
				    	if(data.servant_list.length > 0){
				    		for(var i = 0;data.servant_list.length > i;i++){
				    			var html = "";
				    			
				    			html = "<div class='hov gg'>";
				    			html = html + "<img class='cardbg' src='assets/img/card1.png' alt=''>";
				    			html = html + "<div class='servant'>";
				    			html = html + "<div class='st'><img src='assets/img/icon/item_type/icon_item_type_ac.png' alt=''></div>";
								html = html + "<img class='spanda' src='assets/img/characters/servant/"+cnvs_img(data.servant_list[i].id)+"' alt=''>";
								html = html + "<p class='ct1'>　</p>";
								html = html + "<h1>"+cnvs_name(data.servant_list[i].id)+"</h1>";
								html = html + "<div class='s-rank'>";
								html = html + "<div class='s-rk'><img src='assets/img/s-value1.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_str)+"</p></div> ";
								html = html + "<div class='s-rk'><img src='assets/img/s-value2.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_dex)+"</p></div> ";
								html = html + "<div class='s-rk'><img src='assets/img/s-value3.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_int)+"</p></div> ";
								html = html + "</div>";
								html = html + "</div>";
								html = html + "</div>";
				    			
				    			$(".card-cont").append(html);
				    		}
				    		console.log('servantOK');
				    	}
				    	
				    });

		    		$(".section3").css({"display" :"none"});
		            $(".section3-1").css({"display" :"block"});
		            $(".section3-3").css({"display" :"block"});
		    	}
		    });
		});
	});
}

function gacha(){
	$(".cbtn-1").css({"display" :"none"});
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
			
			$.post("https://dcugl.com:5000/seed", {})
		    .done(function(data){		    		
		    		console.log('data_num -->' + data.num);
		    		console.log('data_seed -->' + data.seed);
		    		console.log('gacha:'+data.num+':'+data.seed);
		    	eos.transfer(account.name, 'unlimittest1', '0.0001 EOS', 'gacha:'+data.num+':'+data.seed, transactionOptions).then(trx => {
					console.log(`Transaction ID: ${trx.transaction_id}`);
				}).catch(error => {
					console.log('error');
				});


		    	$.post("https://dcugl.com:5000/pregacha", { user:account.name })
			    .done(function(data,result_type){
			    	console.log('gacha!!:');
			    	console.log(result_type,data);

			    	if(data != 'Fail to get gacha data'){
			    		
			    		if(result_type == 3){
			    			var html = "";
			    			
			    			html = "<div class='card'>";
			    			html = html + "<div class='card-f'>";
			    			html = html + "<img class='card-f-bg' src='assets/img/card-b.png' alt=''>";
			    			html = html + "<div class='tier'><img src='assets/img/icon/tier/icon_tier_4.png' alt=''></div>";
							html = html + "<div class='tr'><img src='assets/img/icon/item_type/icon_item_type_ac.png' alt=''></div>";
							html = html + "<img class='item-b' src='assets/img/characters/item/tier3_HeavyA.png' alt=''>";
							html = html + "<p>Legendary</p>";
							html = html + "<h1>GOLD PANDA</h1>";
							html = html + "<div class='rank'>";
							html = html + "<div class='rk'><img src='assets/img/value1.png' alt=''><span>S+</span></div>";
							html = html + "</div>";
							html = html + "</div>";
							html = html + "<div class='card-b'><img src='assets/img/card-b.png' alt=''></div>";							
							html = html + "<div class='cbtn-2'><a href='javascript:gacha_ok();'><p>OK</p></a></div>";
							html = html + "</div>";
			    			
			    			$(".sec3-2").append(html);
			    		}
			    		if(result_type == 2){
			    			var html = "";
			    			
			    			html = "<div class='card'>";
			    			html = html + "<div class='card-f'>";
			    			html = html + "<img class='monster-b' src='assets/img/mst-panda-gold.png' alt=''>";
			    			html = html + "<p>Legendary</p>";
							html = html + "<h1>GOLD PANDA</h1>";
							html = html + "<div class='rank'>";
							html = html + "<div class='rk'><img src='assets/img/value1.png' alt=''><span>S+</span></div>";
							html = html + "<div class='rk'><img src='assets/img/value1.png' alt=''><span>S+</span></div>";
							html = html + "<div class='rk'><img src='assets/img/value1.png' alt=''><span>S+</span></div>";
							html = html + "</div>";
							html = html + "</div>";
							html = html + "<div class='card-b'><img src='assets/img/card-b.png' alt=''></div>";
							html = html + "<div class='cbtn-2'><a href='javascript:gacha_ok();'><p>OK</p></a></div>";
							html = html + "</div>";				
			    			
			    			$(".sec3-2").append(html);
			    		}
			    		if(result_type == 1){
			    			var html = "";
			    			
			    			html = "<div class='card'>";
			    			html = html + "<div class='card-f'>";
			    			html = html + "<img class='card-f-bg' src='assets/img/card-b.png' alt=''>";
			    			html = html + "<div class='tr'><img src='assets/img/icon/item_type/icon_item_type_ac.png' alt=''></div>";
							html = html + "<img class='servant-b' src='assets/img/characters/servant/servant_cleric_KF.png' alt=''>";
							html = html + "<p class='ct1'>　</p>";
							html = html + "<h1></h1>";
							html = html + "<div class='s-rank'>";
							html = html + "<div class='s-rk'><img src='assets/img/s-value1.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_str)+"</p></div> ";
							html = html + "<div class='s-rk'><img src='assets/img/s-value2.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_dex)+"</p></div> ";
							html = html + "<div class='s-rk'><img src='assets/img/s-value3.png' alt=''><p>"+cnvs_rank(data.servant_list[i].status.basic_int)+"</p></div> ";
							html = html + "</div>";
							html = html + "</div>";
							html = html + "<div class='card-b'><img src='assets/img/card-b.png' alt=''></div>";
							html = html + "<div class='cbtn-2'><a href='javascript:gacha_ok();'><p>OK</p></a></div>";
							html = html + "</div>";
			    			
			    			$(".sec3-2").append(html);
			    		}
			    		
			    		$(".section3-1").css({"display" :"none"});
			    	    $(".sec3-2").addClass("show3-2");
			    	    $(".section3-2").css({"height" :"1080px"});
			    	    $(".section3-2").css({"opacity" :"1"});
			    	}else{
			    		console.log('gacha 거부!');
			    		$(".cbtn-1").css({"display" :"block"});
			    	}
			    });
		    });
		});
	});
}

function gacha_ok(){
	$(".cbtn-1").css({"display" :"block"});
	$(".sec3-2").removeClass("show3-2");
	$(".section3-2").css({"height" :"1px"});
	$(".section3-2").css({"opacity" :"0"});
	$(".section3-1").css({"display" :"block"});	
}
