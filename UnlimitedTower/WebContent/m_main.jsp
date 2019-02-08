<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="UTF-8">
	<title>Unlimitedtower main</title>
	<link rel="stylesheet"  href="m_assets/css/reset.css">
	<link rel="stylesheet"  href="m_assets/css/style.css">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"> 
	
</head>
<body>
	<header id="header">
		<div class="container">
	        <div class="row">
	            <div class="header">
	                <div class="logo"><img src="m_assets/img/logo.png" alt="logo"></div>
	                <div class="icon-wrap"">
						<div class="icon"></div>
					</div>
	                <div class="cate">
	                  	<ul>
	                  		<li><a href="javascript:goMenu('2')" >About</a></li>
	                  		<li><a href="javascript:goMenu('5')" class="cate-1">Roadmap</a></li>
	                  		<li><a href="javascript:goMenu('6')" class="cate-2">Partners</a></li>
	                  		<li><a href="javascript:goMenu('7')" class="cate-3">Contact</a></li>
	                  	</ul>
	                </div> 
	            </div>
	        </div>
    	</div>
    </header>
	
	<div class="section">
	<!--section1-->	
		<div id="section1">
			<div class="section1">
				<div class="container">
					<div class="sec1">
						<div class="sec-l">
							<div class="main-bg"><img src="m_assets/img/group-6.png" alt=""><img src="m_assets/img/group-6-copy.png" alt=""></div>
								<div class="tower">
									<div class="ov">
									<div class="tower1"><img src="m_assets/img/tower-m.png" alt=""></div>
								</div>
							</div>	
						</div>
						<div class="sec-r">
							<img class="main-p" src="m_assets/img/mbanner-icon.png" alt="">
							<img class="main-bitmap" src="m_assets/img/bitmap.png" alt="">
						</div>
					</div>
				</div>
			</div>
		</div>
	<!--section1-->
	<%@include file = "m_section/m_section2.jsp"%>
	<%@include file = "m_section/m_section3.jsp"%>	
	<%@include file = "m_section/m_section4.jsp"%>
	<%@include file = "m_section/m_section5.jsp"%>
	<%@include file = "m_section/m_section6.jsp"%>
	</div>
	
	<footer id="section7">
		<div class="container">
			<div class="row">
				<div class="footer">
					<div class="f-logo"><img src="m_assets/img/devtooth-logo-white.png"></div>
					<div class="sns">
						<a href="#"><img src="m_assets/img/contact-linkedin-icon.png"></a>
                      	<a href="#"><img src="m_assets/img/contact-facebook-icon.png"></a>
                      	<a href="#"><img src="m_assets/img/github.png"></a>
                      	<a href="#"><img src="m_assets/img/kakaotalk.png"></a>
                      	<a href="#"><img src="m_assets/img/telegram.png"></a>   
                      	<a href="#"><img src="m_assets/img/messn.png"></a> 
					</div>
					<div class="about">
						<ul>
							<li>위치 : 서울 강남구 선릉로 93길 27 8층</li>
							<li>메일 : tom@devtooth.com</li>
							<li>전화번호 : 010-8360-5208</li>
						</ul>
						<em>copytright ⓒ 2018Devtooth. All Rights Reserved.</em>
					</div>
				</div>
			</div>
		</div>
	</footer>
<script src="m_assets/js/jquery-1.12.4.js"></script>
<script src="m_assets/js/jquery-migrate-1.2.1.min.js"></script>
<script src="m_assets/js/jquery-ui-1.12.1.min.js"></script>
<script src="m_assets/js/event.js?ver=2"></script>
<script src="m_assets/js/html5shiv.js"></script>
</body>
</html>