<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<!--header-->
<header id="header">
	<div class="row">
		<div class="header">
			<div class="logo">
				<a href="javascript:goMenu('1')"><img src="assets/img/hlogo.png" alt=""></a>
			</div>
			<div class="icon-wrap"">
				<div class="icon"></div>
			</div>
			<div class="header-r">
				<div class="cate">
					<ul>
						<li><a href="javascript:goMenu('2')" >Video</a></li>
						<li><a href="javascript:goMenu('3')" class="cate-1">Pre-Register</a></li>
						<li><a href="javascript:goMenu('4')" class="cate-2">Gacha</a></li>
						<li><a href="javascript:goMenu('5')" class="cate-3">How to play</a></li>
						<li><a href="javascript:goMenu('6')" class="cate-3">Roadmap</a></li>
						<li><a href="javascript:goMenu('7')" class="cate-4">Partner</a></li>
						<li><a href="javascript:goMenu('8')" class="cate-3">Contact</a></li>
					</ul>
				</div>
				<div class="language">
	            <%if(country.equals("en")){ %>
	                <div class="En"><a href="javascript:goCountry();"><img src="assets/img/En.png" alt=""><span>English</span></a></div>
	            <%}else if(country.equals("ko")){ %>
	                <div class="ko"><a href="javascript:goCountry();"><img src="assets/img/Ko.png" alt=""><span>한국어</span></a></div>
	        	<%}else if(country.equals("ch")){ %>
	                <div class="ch"><a href="javascript:goCountry();"><img src="assets/img/china.png" alt=""><span>中文</span></a></div>
	            <%} %>
	                <ul class="lan-cate">
	                    <li><a href="javascript:country_action('en');"><img src="assets/img/En.png" alt=""><span>English</span></a></li>
	                    <li><a href="javascript:country_action('ko');"><img src="assets/img/Ko.png" alt=""><span>한국어</span></a></li>
	                    <li><a href="javascript:country_action('ch');"><img src="assets/img/china.png" alt=""><span>中文</span></a></li>
	                </ul>
	            </div>
				<div class="login"><a href="javascript:scatter_login();">Scatter</a></div>
				<div class="name"></div>
				<div class="logout"><a href="javascript:scatter_logout();">Logout</a></div>
			</div>
		</div>
	</div>
</header>
<!--header-->
