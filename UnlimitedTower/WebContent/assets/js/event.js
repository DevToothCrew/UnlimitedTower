
//cate go
function goMenu(seq){       
	var offset = $("#section" + seq).offset();
	$('html, body').animate({scrollTop : offset.top}, 400); 
}
  
//slide animation
function goCountry(){
    $(".lan-cate").slideToggle();
}
$(".section").click(function(){;
    $(".lan-cate").slideUp();
});
$(".login").click(function(){;
    $(".lan-cate").slideUp();
});
function country_action(str){
	document.main.country.value = str;
	
	document.main.submit();
}

function name_click(){
	$(".logout").slideToggle();
}
$(".section").click(function(){;
    $(".logout").slideUp();
});
$(".lan-cate").click(function(){;
    $(".logout").slideUp();
});


//scroll down animation
$(function() {
    function swing() {
        $('.scroll').animate({'top':'0px'},600).animate({'top':'5px'},600, swing);
    }
    swing();
});

//icon hover click active 
$(function() { 
    $(".sec1-sns li a img").hover(function(){ 
        $(this).attr("src", $(this).attr("src").replace(".png", "-h.png")); 
    }, function(){ 
        $(this).attr("src", $(this).attr("src").replace("-h.png", ".png")); 
    });  
});
$(function () {
    $('.sec1-sns li:eq(0) a img').mousedown(function () {
        $('.f-sns li:eq(0) a img').attr('src', 'assets/img/twitter-p.png');
    });
    $('.sec1-sns li:eq(0) a img').mouseup(function () {
        $('.f-sns li:eq(0) a img').attr('src', 'assets/img/twitter.png');
    });
    //eq : 0과 같다면 (index번호) img의 src 속성값을 2.jpg로 바꿔라
    $('.sec1-sns li:eq(1) a img').mousedown(function () {
        $('.sec1-sns li:eq(1) a img').attr('src', 'assets/img/tele(en)-p.png');
    });
    $('.sec1-sns li:eq(1) a img').mouseup(function () {
        $('.sec1-sns li:eq(1) a img').attr('src', 'assets/img/tele(en).png');
    });
    $('.sec1-sns li:eq(2) a img').mousedown(function () {
        $('.sec1-sns li:eq(2) a img').attr('src', 'assets/img/telegram-p.png');
    });
    $('.sec1-sns li:eq(2) a img').mouseup(function () {
        $('.sec1-sns li:eq(2) a img').attr('src', 'assets/img/telegram.png');
    });
    $('.sec1-sns li:eq(3) a img').mousedown(function () {
        $('.sec1-sns li:eq(3) a img').attr('src', 'assets/img/kakao-p.png');
    });
    $('.sec1-sns li:eq(3) a img').mouseup(function () {
        $('.sec1-sns li:eq(3) a img').attr('src', 'assets/img/kakao-png');
    });
    $('.sec1-sns li:eq(4) a img').mousedown(function () {
        $('.sec1-sns li:eq(4) a img').attr('src', 'assets/img/md-p.png');
    });
     $('.sec1-sns li:eq(4) a img').mouseup(function () {
        $('.sec1-sns li:eq(4) a img').attr('src', 'assets/img/md.png');
    });
        $('.sec1-sns li:eq(5) a img').mousedown(function () {
        $('.sec1-sns li:eq(5) a img').attr('src', 'assets/img/github-p.png');
    });
      $('.sec1-sns li:eq(5) a img').mouseup(function () {
        $('.sec1-sns li:eq(5) a img').attr('src', 'assets/img/github.png');
    });
        $('.sec1-sns li:eq(6) a img').mousedown(function () {
        $('.sec1-sns li:eq(6) a img').attr('src', 'assets/img/yt-p.png');
    });
     $('.sec1-sns li:eq(6) a img').mouseup(function () {
        $('.sec1-sns li:eq(6) a img').attr('src', 'assets/img/yt.png');
    });
});
$(function() { 
    $(".f-sns li a img").hover(function(){ 
        $(this).attr("src", $(this).attr("src").replace(".png", "-h.png")); 
    }, function(){ 
        $(this).attr("src", $(this).attr("src").replace("-h.png", ".png")); 
    }); 
});

$(function () {
    $('.f-sns li:eq(0) a img').mousedown(function () {
        $('.f-sns li:eq(0) a img').attr('src', 'assets/img/twitter-p.png');
    });
    $('.f-sns li:eq(0) a img').mouseup(function () {
        $('.f-sns li:eq(0) a img').attr('src', 'assets/img/twitter.png');
    });
    //eq : 0과 같다면 (index번호) img의 src 속성값을 2.jpg로 바꿔라
    $('.f-sns li:eq(1) a img').mousedown(function () {
        $('.f-sns li:eq(1) a img').attr('src', 'assets/img/tele(en)-p.png');
    });
    $('.f-sns li:eq(1) a img').mouseup(function () {
        $('.f-sns li:eq(1) a img').attr('src', 'assets/img/tele(en).png');
    });
    $('.f-sns li:eq(2) a img').mousedown(function () {
        $('.f-sns li:eq(2) a img').attr('src', 'assets/img/telegram-p.png');
    });
    $('.f-sns li:eq(2) a img').mouseup(function () {
        $('.f-sns li:eq(2) a img').attr('src', 'assets/img/telegram.png');
    });
    $('.f-sns li:eq(3) a img').mousedown(function () {
        $('.f-sns li:eq(3) a img').attr('src', 'assets/img/kakao-p.png');
    });
    $('.f-sns li:eq(3) a img').mouseup(function () {
        $('.f-sns li:eq(3) a img').attr('src', 'assets/img/kakao.png');
    });
    $('.f-sns li:eq(4) a img').mousedown(function () {
        $('.f-sns li:eq(4) a img').attr('src', 'assets/img/md-p.png');
    });
    $('.f-sns li:eq(4) a img').mouseup(function () {
        $('.f-sns li:eq(4) a img').attr('src', 'assets/img/md.png');
    });
    $('.f-sns li:eq(5) a img').mousedown(function () {
        $('.f-sns li:eq(5) a img').attr('src', 'assets/img/github-p.png');
    });
    $('.f-sns li:eq(5) a img').mouseup(function () {
        $('.f-sns li:eq(5) a img').attr('src', 'assets/img/github.png');
    });
    $('.f-sns li:eq(6) a img').mousedown(function () {
        $('.f-sns li:eq(6) a img').attr('src', 'assets/img/yt-p.png');
    });
    $('.f-sns li:eq(6) a img').mouseup(function () {
        $('.f-sns li:eq(6) a img').attr('src', 'assets/img/yt.png');
    });
    $('.f-sns li:eq(7) a img').mousedown(function () {
        $('.f-sns li:eq(7) a img').attr('src', 'assets/img/mail-p.png');
    });
    $('.f-sns li:eq(7) a img').mouseup(function () {
        $('.f-sns li:eq(7) a img').attr('src', 'assets/img/mail.png');
    });
});




// 스크롤 애니메이션 
$(window).scroll(function(){
    var wScroll = $(this).scrollTop();

    if(wScroll >= $(".sec2").offset().top -$(window).height()/1.1){
        $(".sec2").addClass("show2");
    }else {
        $(".sec2").removeClass("show2");
    }
    if(wScroll >= $(".sec3").offset().top -$(window).height()/1.1){
        $(".sec3").addClass("show3");
    }else {
        $(".sec3").removeClass("show3");
    }
    if(wScroll >= $(".sec4").offset().top -$(window).height()/1.1){
        $(".sec4").addClass("show4");
    }else {
        $(".sec4").removeClass("show4");
    }
    if(wScroll >= $(".sec5").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5");
    }else {
        $(".sec5").removeClass("show5");
    }
    if(wScroll >= $(".explain1").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5-1");
    }else {
        $(".sec5").removeClass("show5-1");
    }
    if(wScroll >= $(".explain2").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5-2");
    }else {
        $(".sec5").removeClass("show5-2");
    }
    if(wScroll >= $(".explain3").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5-3");
    }else {
        $(".sec5").removeClass("show5-3");
    }
    if(wScroll >= $(".sec6").offset().top -$(window).height()/1.1){
        $(".sec6").addClass("show6");
    }else {
        $(".sec6").removeClass("show6");
    }
});

/*tower animation*/
$(".section1").ready(function(){
    loop();
});
function loop(){
    $(".tower1").animate({ top: "-1120%" },2000)
                .delay(3000).animate({ top: "-1100%" },2000)
                .delay(3000).animate({ top: "-1080%" },2000)
                .delay(3000).animate({ top: "-1060%" },2000)
                .delay(3000).animate({ top: "-1040%" },2000)
                .delay(3000).animate({ top: "-1020%" },2000)
                .delay(3000).animate({ top: "-1000%" },2000)
                .delay(3000).animate({ top: "-980%" },2000)
                .delay(3000).animate({ top: "-960%" },2000)
                .delay(3000).animate({ top: "-940%" },2000)
                .delay(3000).animate({ top: "-920%" },2000)
                .delay(3000).animate({ top: "-900%" },2000)
                .delay(3000).animate({ top: "-880%" },2000)
                .delay(3000).animate({ top: "-860%" },2000)
                .delay(3000).animate({ top: "-840%" },2000)           
                .delay(3000).animate({ top: "-820%" },2000)
                .delay(3000).animate({ top: "-800%" },2000)
                .delay(3000).animate({ top: "-780%" },2000)
                .delay(3000).animate({ top: "-760%" },2000)
                .delay(3000).animate({ top: "-740%" },2000)
                .delay(3000).animate({ top: "-720%" },2000)
                .delay(3000).animate({ top: "-700%" },2000)
                .delay(3000).animate({ top: "-680%" },2000)
                .delay(3000).animate({ top: "-660%" },2000)
                .delay(3000).animate({ top: "-640%" },2000)
                .delay(3000).animate({ top: "-620%" },2000)
                .delay(3000).animate({ top: "-600%" },2000)
                .delay(3000).animate({ top: "-580%" },2000)               
    $(".tower1").delay(3000).animate({ top: "-560%" },2000)
                .delay(3000).animate({ top: "-540%" },2000)
                .delay(3000).animate({ top: "-520%" },2000)
                .delay(3000).animate({ top: "-500%" },2000)
                .delay(3000).animate({ top: "-480%" },2000)
                .delay(3000).animate({ top: "-460%" },2000)
                .delay(3000).animate({ top: "-440%" },2000)
                .delay(3000).animate({ top: "-420%" },2000)
                .delay(3000).animate({ top: "-400%" },2000)
                .delay(3000).animate({ top: "-380%" },2000)
                .delay(3000).animate({ top: "-360%" },2000)
                .delay(3000).animate({ top: "-340%" },2000)
                .delay(3000).animate({ top: "-320%" },2000)
                .delay(3000).animate({ top: "-300%" },2000)
                .delay(3000).animate({ top: "-280%" },2000)
                .delay(3000).animate({ top: "-260%" },2000)
                .delay(3000).animate({ top: "-240%" },2000)
                .delay(3000).animate({ top: "-220%" },2000)
                .delay(3000).animate({ top: "-200%" },2000)
                .delay(3000).animate({ top: "-180%" },2000)
                .delay(3000).animate({ top: "-160%" },2000)
                .delay(3000).animate({ top: "-140%" },2000)
                .delay(3000).animate({ top: "-120%" },2000,loop)
}



$('.slide-wrap').slick({
    dots: true,
    slidesToShow: 5,
    slidesToScroll: 5,
});

