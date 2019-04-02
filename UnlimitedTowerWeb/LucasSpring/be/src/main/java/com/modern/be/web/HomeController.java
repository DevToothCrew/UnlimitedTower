package com.modern.be.web;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.RequestMapping;

@Controller
public class HomeController {
	@RequestMapping("/kr")
	public String index_kr(Model model) {
		model.addAttribute("lenguage", "kr");
		return "index";
	}
	@RequestMapping("/en")
	public String index_en(Model model) {
		model.addAttribute("lenguage", "kr");
		return "index";
	}
	@RequestMapping("/ch")
	public String index_ch(Model model) {
		model.addAttribute("lenguage", "kr");
		return "index";
	}
}
