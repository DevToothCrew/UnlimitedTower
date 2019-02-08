
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

function goMenu(seq){       
        var offset = $("#section" + seq).offset();
        $('html, body').animate({scrollTop : offset.top}, 400); 
    }
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
