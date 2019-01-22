const scatterEOS = window.ScatterEOS;
const scatterJS = window.ScatterJS;



function scatter_login(){
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
		    	if(data.signup == null){
		    		console.log('tiger');
		    		eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'presignup:'+login_num+':'+login_seed, transactionOptions).then(trx => {
						console.log(`Transaction ID: ${trx.transaction_id}`);
					}).catch(error => {
							console.log('error');
					});
		    		
		    		$.post("https://dcugl.com:5000/presignup", { user:account.name })
				    .done(function(data){
				    	console.log(data);
				    });
		    	}else{
		    		$.post("https://dcugl.com:5000/prelogin", { user:account.name })
				    .done(function(data){
				    	console.log('else : ' + data);
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
		    	eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'gacha:'+data.num+':'+data.seed, transactionOptions).then(trx => {
					console.log(`Transaction ID: ${trx.transaction_id}`);
				}).catch(error => {
					console.log('error');
				});


		    	$.post("https://dcugl.com:5000/pregacha", { user:account.name })
			    .done(function(data){
			    	console.log('gacha!!:');
			    	console.log(data);

			    	if(data != 'Fail to get gacha data'){
			    		$(".section3-1").css({"display" :"none"});
			    	    $(".sec3-2").addClass("show3-2");
			    	    $(".section3-2").css({"height" :"1080px"});
			    	    $(".section3-2").css({"opacity" :"1"});
			    	}else{
			    		console.log('gacha 거부!');
			    	}
			    });
		    });
		});
	});
}

function gacha_ok(){
	$(".sec3-2").removeClass("show3-2");
	$(".section3-2").css({"height" :"1px"});
	$(".section3-2").css({"opacity" :"0"});
	$(".section3-1").css({"display" :"block"});
	$(".section3-3").css({"display" :"block"});
}
