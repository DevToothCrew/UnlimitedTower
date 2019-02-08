<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<script type="text/javascript">
	<% String isMobile = "";%>
	var uAgent = navigator.userAgent.toLowerCase();
	
	// 모바일 기기 추가시 등록
	var mobilePhones = new Array('iphone', 'ipod', 'android', 'blackberry', 'windows ce', 'nokia', 'webos', 'opera mini', 'sonyericsson', 'opera mobi', 'iemobile', 'windows phone');
	 
	var isMobile = false;
	 
	for( var i = 0 ; i < mobilePhones.length ; ++i ){
	    if( uAgent.indexOf(mobilePhones[i]) > -1){
	        isMobile = true;
	         
	        break;
	    }
	}
	 
	// 모바일인 경우
	if( isMobile ){
		<% isMobile = "M"; %>
	}else{
		<% isMobile = "P"; %>
	}
</script>
<html id="index" lang="en">
<%if(isMobile.equals("M")){ %>
<%@include file = "m_main.jsp"%>
<%}else{ %>
<%@include file = "main.jsp"%>
<%} %>
</html>