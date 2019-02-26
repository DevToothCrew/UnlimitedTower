<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!--popup-->
	<div id="m-popup">
		<div class="m-popup">
	        <div class="m-container">
	        	<div class="m-pop">
	        		<div class="event-tit">
	        			<% if(country.equals("en")){ %>
	        			<h1><img src="assets/img/tit.png" alt=""></h1>
	        			<em><strong>Date</strong>2018.02.13 ~ 2018.04.01</em>
	        			<span>After the sign-in, press the attendance button at the bottom to complete the participation! Compensation will be given according to the number of attendance! Attendance days will be updated at 0:00 AM on UTC + 0.</span>
	        			<p>* Only pre-registered accounts can participate in the event.</p>
	        			<% }else if(country.equals("ko")){ %>
	        			<h1><img src="assets/img/tit.png" alt=""></h1>
	        			<em><strong>Date</strong>2018.02.25 ~ 2018.04.01</em>
	        			<span>스캐터로 로그인 하신 후 하단의 출석하기 버튼을 누르면 참여 완료!!<br>출석 횟수에 따라 다양한 보상을 드립니다!<br>출석 일수는 UTC+0 기준 0:00 AM에 갱신됩니다.</span>
	        			<p>*사전 등록을 마친 계정만 이벤트에 참여 가능합니다.</p>
	        			<% }else if(country.equals("ch")){ %>
	        			<h1><img src="assets/img/ch-tit.png" alt=""></h1>
	        			<em><strong>Date</strong>2018.02.13 ~ 2018.04.01</em>
	        			<span>以分散方式登录后，单击底部的“出勤”按钮并加入！<br>将根据出席人数给予补偿！<br>出勤日期将在UTC + 0的凌晨0:00更新。</span>
	        			<p>*只有预先注册的帐户才能参加活动。.</p>
	        			<%} %>
	        			<div class="check-icon"><a href="javascript:closePopup();"><img src="assets/img/close-icon.png" alt=""></a></div>
	        		</div>
	        		<div class="event-cont">
	        			<div class="date-wrap">
		        			<div id="day1" class="date">
		        				<h2>Day 1</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day2" class="date">
		        				<h2>Day 2</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day3" class="date">
		        				<h2>Day 3</h2>
		        				<img src="assets/img/c-servant1.png" alt="">
		        				<p>Servant</p>
		        			</div>
		        			<div id="day4" class="date">
		        				<h2>Day 4</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day5" class="date">
		        				<h2>Day 5</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day6" class="date">
		        				<h2>Day 6</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>150UTG</p>
		        			</div>
		        			<div id="day7" class="date">
		        				<h2>Day 7</h2>
		        				<img src="assets/img/c-monster1.png" alt="">
		        				<p>Monster</p>
		        			</div>
		        			<div id="day8" class="date">
		        				<h2>Day 8</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day9" class="date">
		        				<h2>Day 9</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day10" class="date">
		        				<h2>Day 10</h2>
		        				<img src="assets/img/c-item1.png" alt="">
		        				<p>Item</p>
		        			</div>
		        			<div id="day11" class="date">
		        				<h2>Day 11</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day12" class="date">
		        				<h2>Day 12</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day13" class="date">
		        				<h2>Day 13</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>150UTG</p>
		        			</div>
		        			<div id="day14" class="date">
		        				<h2>Day 14</h2>
		        				<img src="assets/img/free-gacha.png" alt="">
		        				<p>1Free Gacha</p>
		        			</div>
		        			<div id="day15" class="date">
		        				<h2>Day 15</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day16" class="date">
		        				<h2>Day 16</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day17" class="date">
		        				<h2>Day 17</h2>
		        				<img src="assets/img/c-servant2.png" alt="">
		        				<p>Servant</p>
		        			</div>
		        			<div id="day18" class="date">
		        				<h2>Day 18</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day19" class="date">
		        				<h2>Day 19</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day20" class="date">
		        				<h2>Day 20</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>150UTG</p>
		        			</div>
		        			<div id="day21" class="date">
		        				<h2>Day 21</h2>
		        				<img src="assets/img/c-monster2.png" alt="">
		        				<p>Monster</p>
		        			</div>
		        			<div id="day22" class="date">
		        				<h2>Day 22</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day23" class="date">
		        				<h2>Day 23</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day24" class="date">
		        				<h2>Day 24</h2>
		        				<img src="assets/img/c-item2.png" alt="">
		        				<p>Item</p>
		        			</div>
		        			<div id="day25" class="date">
		        				<h2>Day 25</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>50UTG</p>
		        			</div>
		        			<div id="day26" class="date">
		        				<h2>Day 26</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>100UTG</p>
		        			</div>
		        			<div id="day27" class="date">
		        				<h2>Day 27</h2>
		        				<img src="assets/img/UTGcoin.png" alt="">
		        				<p>150UTG</p>
		        			</div>
		        			<div id="day28" class="date">
		        				<h2>Day 28</h2>
		        				<img src="assets/img/free-gacha.png" alt="">
		        				<p>1Free Gacha</p>
		        			</div>
		        			<div id="day1" class="date">  
		        				<h2></h2>
		        			</div>
		        			<div id="day1" class="date">
		        				<h2></h2>
		        			</div>
		        		</div>
	        		</div>
        			<div class="event-btn">
        				<% if(country.equals("en")){ %>
	        			<a href="javascript:daily_check();"><p >Check-in Now!</p></a>
	        			<% }else if(country.equals("ko")){ %>
	        			<a href="javascript:daily_check();"><p >Check-in Now!</p></a>
	        			<% }else if(country.equals("ch")){ %>
	        			<a href="javascript:daily_check();"><p >现在出席</p></a>
	        			<%} %>
        			</div>
					<div class="pop-add">
						<div class="add-cont">

							<p>100 UTG 획득!</p>
							<div class="add-btn"><a href="">OK</a></div>
						</div>
					</div>
	        	</div>
	        </div>
	    </div>
	</div>
<!--popup-->