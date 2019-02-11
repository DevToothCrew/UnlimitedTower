<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<% 
	String isMobile = "";
	
	String userAgent = request.getHeader("user-agent");
	System.out.println("userAgent"+userAgent);
	String[] browser = {"iPhone", "iPod","Android", "blackberry", "windows ce", "nokia", "webos", "opera mini", "sonyericsson", "opera mobi", "iemobile", "windows phone", "SAMSUNG", "Mobile Safari"};					
	
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
<html id="index" lang="en">
<%if(isMobile.equals("M")){ System.out.println("M : "+isMobile); %>
<%@include file = "m_main.jsp"%>
<%}else{ System.out.println("P : "+isMobile); %>
<%@include file = "main.jsp"%>
<%} %>
</html>