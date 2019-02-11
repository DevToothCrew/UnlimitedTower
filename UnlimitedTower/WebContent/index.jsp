<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<% 
	String isMobile = "";
	
	String userAgent = request.getHeader("user-agent");
	System.out.println("userAgent"+userAgent);
	String[] browser = {"iPhone", "iPod","Android", "blackberry", "windows ce", "nokia", "webos", "opera mini", "sonyericsson", "opera mobi", "iemobile", "windows phone"};
	
	for (int i = 0; i < browser.length; i++) {
		if(userAgent.matches(".*"+browser[i]+".*")){
			isMobile = "M";
			break;
		}else{
			isMobile = "P";
			break;
		}
	}
%>
<script type="text/javascript">
	
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
		
	}else{
		
	}
</script>
<html id="index" lang="en">
<%if(isMobile.equals("M")){ System.out.println("M : "+isMobile); %>
<%@include file = "m_main.jsp"%>
<%}else{ System.out.println("P : "+isMobile); %>
<%@include file = "main.jsp"%>
<%} %>
</html>