function goMenu(seq){
       var offset = $("#section" + seq).offset();
       $('html, body').animate({scrollTop : offset.top}, 400);
   }

$(".icon-wrap").click(function(){
	$(".icon").toggleClass("animation");
	$(".cate").slideToggle();
});

$(".section").click(function(){;
	$(".icon").toggleClass("animation");
	$(".cate").slideUp();
});

$(".cate ul li").click(function(){;
	$(".icon").toggleClass("animation");
	$(".cate").slideUp();
});

$(window).resize(function(){
	var wWidth =$(window).width();

	//화면 크기가 960이상일 떄 style="display:none;" 삭제
	if(wWidth >960 && $(".menu").is(":hidden")){
		$(".menu").removeAttr("style")
	}
});
	
//banner tower anmation
$("tower").ready(function(){
    $(".tower1").animate({ top: "-188%" },2000)
    $(".tower1").delay(500).animate({ top: "-173%" },2000)
    $(".tower1").delay(500).animate({ top: "-158%" },2000)
    $(".tower1").delay(500).animate({ top: "-143%" },2000)
    $(".tower1").delay(500).animate({ top: "-128%" },2000)
    $(".tower1").delay(500).animate({ top: "-113%" },2000)
    $(".tower1").delay(500).animate({ top: "-98%" },2000)
    $(".tower1").delay(500).animate({ top: "-83%" },2000)
    $(".tower1").delay(500).animate({ top: "-68%" },2000)
    $(".tower1").delay(500).animate({ top: "-53%" },2000)
    $(".tower1").delay(500).animate({ top: "-33%" },2000)
    $(".tower1").delay(500).animate({ top: "-18%" },2000)
    $(".tower1").delay(500).animate({ top: "-3%" },2000)
    $(".tower1").delay(500).animate({ top: "-180%" },2000);
});



// 스크롤 애니메이션 
$(window).scroll(function(){
    var wScroll = $(this).scrollTop();

    if(wScroll >= $(".sec1").offset().top -$(window).height()/1.1){
        $(".sec1").addClass("show2");
    }else {
        $(".sec1").removeClass("show2");
    }
    if(wScroll >= $(".sec3").offset().top -$(window).height()/1.1){
        $(".sec3").addClass("show3");
    }else {
        $(".sec3").removeClass("show3");
    }
    if(wScroll >= $(".explain1").offset().top -$(window).height()/1.1){
        $(".explain1").addClass("show3-1");
    }else {
        $(".explain1").removeClass("show3-1");
    }
    if(wScroll >= $(".explain2").offset().top -$(window).height()/1.1){
        $(".explain2").addClass("show3-2");
    }else {
        $(".explain2").removeClass("show3-2");
    }
    if(wScroll >= $(".explain3").offset().top -$(window).height()/1.1){
        $(".explain3").addClass("show3-3");
    }else {
        $(".explain3").removeClass("show3-3");
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
    if(wScroll >= $(".sec6").offset().top -$(window).height()/1.1){
        $(".sec6").addClass("show6");
    }else {
        $(".sec6").removeClass("show6");
    }
});

//section3 move animation
var $wrap = $(".move");
var x = 0;
var y = 0;
var folloxX = 0;
var folloxY = 0;
var friction = 1/6;

function animate(){
    x += (folloxX - x) * friction;
    y += (folloxY - y) * friction;
    
    $wrap.css({
    'transform': 'translate(-50%, -50%) perspective(600px) rotatey('+ -x +'deg) rotatex('+ y +'deg)'
    });
    window.requestAnimationFrame(animate);
}


$(window).on('mousemove click',function(e){
    var iMouseX = Math.max(-100, Math.min(100, $(window).width()/2 - e.clientX));
    var iMouseY = Math.max(-100, Math.min(100, $(window).height()/2 - e.clientY));
    folloxX = (1 * iMouseX) / 100;
    folloxY = (1 * iMouseY) / 100;
});

animate();
//section3 클릭시 넘어감
$(".cbtn a").click(function(){ 
        $(".section3").css({"display" :"none"}); 
        $(".section3-1").css({"display" :"block"}); 
});
$(".cbtn-1 a").click(function(){ 
        $(".section3-1").css({"display" :"none"}); 
        $(".section3-2").css({"display" :"block"}); 
});
$(".cbtn-2 a").click(function(){ 
        $(".section3-2").css({"display" :"none"}); 
        $(".section3-3").css({"display" :"block"}); 
});


//section3-3 hover


$(".card1").hover(function(){
    $(".card-cont2").find(".card1-1").css({"display" :"block"});
},function(){
    $(".card-cont2").find(".card1-1").css({"display" :"none"});
});

//map 부분 애니메이션
$(".map-next").on("click",function(){
    $(".map-cont")
        .animate({ left:"-150%" },10000)
        .animate({ left:"0%" },1000)
});
