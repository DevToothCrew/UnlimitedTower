
//cate go
function goMenu(seq){       
	var offset = $("#section" + seq).offset();
	$('html, body').animate({scrollTop : offset.top}, 400); 
}
  
$(".icon-wrap").click(function(){
	$(".icon").toggleClass("animation");
	$(".header-r").toggleClass("open");
});

$(".section").click(function(){;
	$(".icon").removeClass("animation");
	$(".header-r").removeClass("open");
});

$(".cate ul li").click(function(){;
	$(".icon").removeClass("animation");
	$(".header-r").removeClass("open");
});
  
//slide animation
function goCountry(){
    $(".lan-cate").slideToggle();
    $(".login").toggleClass("margin");
    $(".logout").slideUp();
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
	$(".lan-cate").slideUp();
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
        $('.sec1-sns li:eq(0) a img').attr('src', 'assets/img/twitter-p.png');
    });
    $('.sec1-sns li:eq(0) a img').mouseup(function () {
        $('.sec1-sns li:eq(0) a img').attr('src', 'assets/img/twitter.png');
    });
    //eq : 0과 같다면 (index번호) img의 src 속성값을 2.jpg로 바꿔라
    $('.sec1-sns li:eq(1) a img').mousedown(function () {
        $('.sec1-sns li:eq(1) a img').attr('src', 'assets/img/tele(en)-p.png');
    });
    $('.sec1-sns li:eq(1) a img').mouseup(function () {
        $('.sec1-sns li:eq(1) a img').attr('src', 'assets/img/tele(en).png');
    });
     $('.sec1-sns li:eq(2) a img').mousedown(function () {
        $('.sec1-sns li:eq(2) a img').attr('src', 'assets/img/tele(kr)-p.png');
    });
    $('.sec1-sns li:eq(2) a img').mouseup(function () {
        $('.sec1-sns li:eq(2) a img').attr('src', 'assets/img/tele(kr).png');
    });
    $('.sec1-sns li:eq(3) a img').mousedown(function () {
        $('.sec1-sns li:eq(3) a img').attr('src', 'assets/img/telegram-p.png');
    });
    $('.sec1-sns li:eq(3) a img').mouseup(function () {
        $('.sec1-sns li:eq(3) a img').attr('src', 'assets/img/telegram.png');
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
        $('.f-sns li:eq(2) a img').attr('src', 'assets/img/tele(kr)-p.png');
    });
    $('.f-sns li:eq(2) a img').mouseup(function () {
        $('.f-sns li:eq(2) a img').attr('src', 'assets/img/tele(kr).png');
    });
    $('.f-sns li:eq(3) a img').mousedown(function () {
        $('.f-sns li:eq(3) a img').attr('src', 'assets/img/telegram-p.png');
    });
    $('.f-sns li:eq(3) a img').mouseup(function () {
        $('.f-sns li:eq(3) a img').attr('src', 'assets/img/telegram.png');
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
});





// 스크롤 애니메이션 
$(window).scroll(function(){
    var wScroll = $(this).scrollTop();

    if(wScroll >= $(".sec2").offset().top -$(window).height()/1.1){
        $(".sec2").addClass("show2");
    }
    if(wScroll >= $(".sec3").offset().top -$(window).height()/1.1){
        $(".sec3").addClass("show3");
    }
    if(wScroll >= $(".sec4").offset().top -$(window).height()/1.1){
        $(".sec4").addClass("show4");
    }
    if(wScroll >= $(".sec4-1").offset().top -$(window).height()/1.1){
        $(".sec4-1").addClass("show4-1");
    }
    if(wScroll >= $(".sec5").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5");
    }
    if(wScroll >= $(".explain1").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5-1");
    }
    if(wScroll >= $(".explain2").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5-2");
    }
    if(wScroll >= $(".explain3").offset().top -$(window).height()/1.1){
        $(".sec5").addClass("show5-3");
    }
    if(wScroll >= $(".sec6").offset().top -$(window).height()/1.1){
        $(".sec6").addClass("show6");
    }
    if(wScroll >= $(".sec7").offset().top -$(window).height()/1.1){
        $(".sec7").addClass("show7");
    }
});

$('.slide-wrap').slick({
   dots: false,
   centerMode: true,
   centerPadding: '60px',
   slidesToShow: 1,
 
});

/*tower animation*/
$(".section1").ready(function(){
    loop();
});
function loop(){
    $(".tower1").delay(3000).animate({ top : "+=20%"},2000,loop)

}


