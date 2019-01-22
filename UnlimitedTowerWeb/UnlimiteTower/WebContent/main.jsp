<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html lang="en">
	<head>
	<meta charset="UTF-8">
	<title>UnlimitedTower</title>
	<link rel="stylesheet"  href="assets/css/reset.css">
	<link rel="stylesheet"  href="assets/css/style.css?ver=3">
</head>
<script src="https://cdn.scattercdn.com/file/scatter-cdn/js/latest/scatterjs-core.min.js"></script>
<script src="https://cdn.scattercdn.com/file/scatter-cdn/js/latest/scatterjs-plugin-eosjs.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/eosjs@16.0.9/lib/eos.min.js" crossorigin="anonymous"></script>
<body>
	<header id="header">
		<div class="row">
			<div class="header">
				<div class="logo"><img src="assets/img/bitmap.png" alt="logo"></div>
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
	</header>
	
	<div class="section">
	<!--section1-->
		<div id="section1">
			<div class="section1">
				<div class="container">
		        	<div class="sec1">
						<div class="sec-l">
							<div class="main-bg"><img src="assets/img/group-6.png" alt=""></div>
							<div class="tower">
								<div class="ov">
									<div class="tower1"><img src="assets/img/tower.png" alt=""></div>
								</div>
							</div>	
						</div>
						<div class="sec-r">
							<img class="main-p" src="assets/img/main-invalid-name.png" alt="">
							<img class="main-bitmap" src="assets/img/mbitmap.png" alt="">
						</div>
					</div>
				</div>
		    </div>
	    </div>
	<!--section1-->
	<%@include file = "section/section2.jsp"%>
	<%@include file = "section/section3.jsp"%>
	<%@include file = "section/section3_1.jsp"%>
	<%@include file = "section/section3_2.jsp"%>
	<%@include file = "section/section3_3.jsp"%>
	<%@include file = "section/section4.jsp"%>
	<%@include file = "section/section5.jsp"%>
	<%@include file = "section/section6.jsp"%>
	</div>
	
	<footer id="section7">
		<div class="container">
			<div class="row">
				<div class="footer">
					<div class="f-logo"><img src="assets/img/devtooth-logo-white.png"></div>
					<div class="sns">
						<a href="#"><img src="assets/img/contact-twitter-icon.png"></a>
						<a href="#"><img src="assets/img/contact-linkedin-icon.png"></a>
						<a href="#"><img src="assets/img/contact-facebook-icon.png"></a>
						<a href="#"><img src="assets/img/contact-instagram-icon.png"></a>
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
<script src="assets/js/jquery-1.12.4.js"></script>
<script src="assets/js/jquery-ui-1.12.1.min.js"></script>
<script src="assets/js/event.js?ver=2"></script>
<script src="assets/js/scatter.js?ver=22"></script>
<script src="assets/js/html5shiv.js"></script>
</body>
</html>