import React, { Component } from 'react';
import add from './../assets/img/add.png';
import eboxImg from './../assets/img/eboxImg.png';

class Section2 extends Component {
    render() {
      return (
        <div id="section2">
        <div className="container">
          <div className="section2">
            <div className="sec2">
              <div className="event-wrap">
                <div className="ebox">
                  <div className="ebox-top">
                    <h2 className="ebox-tit">공지사항</h2>
                    <a href="#" className="ebox-add"><img src={add} alt="더보기" /></a>
                  </div>
                  <div className="ebox-bottom">
                    <img src={eboxImg} alt="" className="ebox-img" />
                    <ul>
                      <li><a href="#">공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항</a></li>
                      <li><a href="#">공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항</a></li>
                      <li><a href="#">공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항</a></li>
                    </ul>
                  </div>
                </div>
                <div className="ebox">
                  <div className="ebox-top">
                    <h2 className="ebox-tit">커뮤니티</h2>
                    <a href="#" className="ebox-add"><img src={add} alt="더보기" /></a>
                  </div>
                  <div className="ebox-bottom">
                    <a href="#" className="ebox-img"><img src={eboxImg} alt="" /></a>
                    <ul>
                      <li><a href="#">공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항</a></li>
                      <li><a href="#">공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항</a></li>
                      <li><a href="#">공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항공지사항</a></li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
        );
    }
  }

  
export default Section2;