<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<%
String country = request.getParameter("country");

if(country != null){}else{country = "en";}
%>
<html lang="en">
	<head>
	<meta charset="UTF-8">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<meta name="description" content="Unlimited Tower is the next generation on-chain SRPG Dapp. Upgrade your party and compete at the Unlimited Tower. Win EOS and leave your legacy now!">
	<title>Unlimted Tower - The New Blockchain Battle Dapp</title>
	<link rel="stylesheet"  href="assets/css/reset.css?ver=1">
<% if(country.equals("ch")){ %>	
	<link rel="stylesheet"  href="assets/css/ch_style.css?ver=1">
<% }else{ %>
	<link rel="stylesheet"  href="assets/css/style.css?ver=8">
<% } %>
	<link rel="stylesheet"  href="assets/slick/slick.css"/>
 	<link rel="stylesheet"  href="assets/slick/slick-theme.css"/>
 	<link rel="shortcut icon" href="assets/img/logo32.png">
</head>
<script type="text/javascript">
	window.onload = function(){
		document.main.country.value = '<%=country%>';
		scatter_login();	// scatter.js
	}
</script>
<body>
<form id="main" name="main" action="main.jsp" method="post">
<input type="hidden" id="country" name="country" value="en">
<input type="hidden" id="userId" name="userId" value="">

<% if(country.equals("en")){ %>
<%@include file = "common/header.jsp"%>
	<div class="section">
		<%@include file = "section/en_section1.jsp"%>
		<%@include file = "section/en_section2.jsp"%>
		<%@include file = "section/en_section3.jsp"%>
		<%@include file = "section/en_section4.jsp"%>
		<%@include file = "section/en_section4_1.jsp"%>
		<%@include file = "section/en_section4_3.jsp"%>
		<%@include file = "section/en_section5.jsp"%>
		<%@include file = "section/en_section6.jsp"%>
		<%@include file = "section/en_section7.jsp"%>
	</div>	
<% }else if(country.equals("ko")){ %>
<%@include file = "common/header.jsp"%>
	<div class="section">
		<%@include file = "section/ko_section1.jsp"%>
		<%@include file = "section/ko_section2.jsp"%>
		<%@include file = "section/ko_section3.jsp"%>
		<%@include file = "section/ko_section4.jsp"%>
		<%@include file = "section/ko_section4_1.jsp"%>
		<%@include file = "section/ko_section4_3.jsp"%>
		<%@include file = "section/ko_section5.jsp"%>
		<%@include file = "section/ko_section6.jsp"%>
		<%@include file = "section/ko_section7.jsp"%>
	</div>
<% }else if(country.equals("ch")){ %>
<%@include file = "common/ch_header.jsp"%>
	<div class="section">
		<%@include file = "section/ch_section1.jsp"%>
		<%@include file = "section/ch_section2.jsp"%>
		<%@include file = "section/ch_section3.jsp"%>
		<%@include file = "section/ch_section4.jsp"%>
		<%@include file = "section/ch_section4_1.jsp"%>
		<%@include file = "section/ch_section4_3.jsp"%>
		<%@include file = "section/ch_section5.jsp"%>
		<%@include file = "section/ch_section6.jsp"%>
		<%@include file = "section/ch_section7.jsp"%>
	</div>
<% } %>

<%@include file = "common/footer.jsp"%>
</form>
</body>
<script src="https://cdn.scattercdn.com/file/scatter-cdn/js/latest/scatterjs-core.min.js"></script>
<script src="https://cdn.scattercdn.com/file/scatter-cdn/js/latest/scatterjs-plugin-eosjs.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/eosjs@16.0.9/lib/eos.min.js" crossorigin="anonymous"></script>
<script src="assets/js/jquery-1.12.4.js"></script>
<script src="assets/js/jquery-ui-1.12.1.min.js"></script>
<script src="assets/slick/slick.min.js"></script>
<script src="assets/js/event.js?ver=6"></script>
<script src="assets/js/scatter.js?ver=35"></script>
<script src="assets/js/html5shiv.js"></script>
</html>