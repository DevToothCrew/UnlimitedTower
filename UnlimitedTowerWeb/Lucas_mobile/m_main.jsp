<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<%
String country = (request.getParameter("country") == null) ? "en" : request.getParameter("country");
String ref_id = (request.getParameter("ref_id") == null) ? "" : request.getParameter("ref_id"); 

System.out.println("ref_id : " + ref_id);
if(country != null){}else{country = "en";}
%>
<html lang="en">
	<head>
	<meta charset="UTF-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"> 
	<meta name="description" content="Unlimited Tower is the next generation on-chain SRPG Dapp. Upgrade your party and compete at the Unlimited Tower. Win EOS and leave your legacy now!">
	<title>Unlimited Tower - The New Blockchain Battle Dapp</title>
	<link rel="stylesheet" type="text/css" href="https://fonts.googleapis.com/css?family=Noto+Sans:400,700" rel="stylesheet">
    <link rel="shortcut icon" href="m_assets/img/logo32.png">
	<link rel="stylesheet"  href="m_assets/css/reset.css?ver=3">
	<link rel="stylesheet"  href="m_assets/css/style.css?ver=12">
	<link rel="stylesheet"  href="m_assets/slick/slick.css"/>
 	<link rel="stylesheet"  href="m_assets/slick/slick-theme.css"/>
	</head>
<script type="text/javascript">
	window.onload = function(){
		document.main.country.value = '<%=country%>';
		//scatter_login();	// scatter.js
		
	}
</script>

<body>
<form id="main" name="main" action="m_main.jsp" method="post">
<input type="hidden" id="country" name="country" value="en">
<input type="hidden" id="userId" name="userId" value="">
<% if(country.equals("en")){ %>
<%@include file = "common/m_header.jsp"%>
	<div class="section">	
		<%@include file = "popup/m_popup.jsp"%>
		<%@include file = "m_section/m_en_section1.jsp"%>
		<%@include file = "m_section/m_en_section2.jsp"%>
		<%@include file = "m_section/m_en_section3.jsp"%>
		<%@include file = "m_section/m_en_section4.jsp"%>
		<%@include file = "m_section/m_en_section4_1.jsp"%>
		<%@include file = "m_section/m_en_section4_3.jsp"%>
		<%@include file = "m_section/m_en_section5.jsp"%>
		<%@include file = "m_section/m_en_section6.jsp"%>
		<%@include file = "m_section/m_en_section7.jsp"%>
	</div>	
<% }else if(country.equals("ko")){ %>
<%@include file = "common/m_header.jsp"%>
	<div class="section">	
		<%@include file = "popup/m_popup.jsp"%>
		<%@include file = "m_section/m_ko_section1.jsp"%>
		<%@include file = "m_section/m_ko_section2.jsp"%>
		<%@include file = "m_section/m_ko_section3.jsp"%>
		<%@include file = "m_section/m_ko_section4.jsp"%>
		<%@include file = "m_section/m_ko_section4_1.jsp"%>
		<%@include file = "m_section/m_ko_section4_3.jsp"%>
		<%@include file = "m_section/m_ko_section5.jsp"%>
		<%@include file = "m_section/m_ko_section6.jsp"%>	
		<%@include file = "m_section/m_ko_section7.jsp"%>
	</div>
<% }else if(country.equals("ch")){ %>
<%@include file = "common/m_ch_header.jsp"%>
	<div class="section">
		<%@include file = "popup/m_popup.jsp"%>
		<%@include file = "m_section/m_ch_section1.jsp"%>
		<%@include file = "m_section/m_ch_section2.jsp"%>
		<%@include file = "m_section/m_ch_section3.jsp"%>
		<%@include file = "m_section/m_ch_section4.jsp"%>
		<%@include file = "m_section/m_ch_section4_1.jsp"%>
		<%@include file = "m_section/m_ch_section4_3.jsp"%>
		<%@include file = "m_section/m_ch_section5.jsp"%>
		<%@include file = "m_section/m_ch_section6.jsp"%>
		<%@include file = "m_section/m_ch_section7.jsp"%>	
	</div>
<% } %>	
<%@include file = "common/m_footer.jsp"%>
</form>
</body>
<script src="https://cdn.scattercdn.com/file/scatter-cdn/js/latest/scatterjs-core.min.js"></script>
<script src="https://cdn.scattercdn.com/file/scatter-cdn/js/latest/scatterjs-plugin-eosjs.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/eosjs@16.0.9/lib/eos.min.js" crossorigin="anonymous"></script>
<script src="m_assets/js/jquery-1.12.4.js"></script>
<script src="m_assets/js/jquery-ui-1.12.1.min.js"></script>
<script src="m_assets/slick/slick.min.js"></script>
<script src="m_assets/js/event.js?ver=13"></script>
<script src="m_assets/js/scatter.js?ver=13"></script>
<script src="m_assets/js/html5shiv.js"></script>
</html>