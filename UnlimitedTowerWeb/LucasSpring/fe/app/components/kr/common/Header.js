import React, { Component } from 'react';
import logo from './../assets/img/headerLogo.png';

class Header extends Component {
    render() {
      return (
      <div id="header">
        <div className="info">
          <div className="info-left">
            <div className="link_logo" >
              <img alt="unlimitedtower" className="img_logo" src={logo} />
            </div>
            <div className="link_nftbay"><a href="#">NFT bay</a></div>
            <div className="link_uiss"><a href="#">UISS</a></div>
          </div>
          <div className="info-right">
            <div className="language">
              <a>Language</a>
            </div>
            <div className="login">
              <a href="http://localhost:3001/kr">Login</a>
            </div>
          </div>
        </div>
        <div className="nav">
          <div className="container">
              <ul className="t-cate">
                <li>
                  <a className="link-nav" hhref="#"ref>새소식</a>
                  <ul className="b-cate">
                    <li><a href="http//:localhost:3000/kr/notice">공지사항</a></li>
                    <li><a href="#">이벤트</a></li>
                    <li><a href="#">News</a></li>
                  </ul>
                </li>
                <li>
                  <a className="link-nav" href="#">게임소개</a>
                  <ul className="b-cate">
                    <li><a href="#">게임소개</a></li>
                    <li><a href="#">Servant</a></li>
                    <li><a href="#">Monster</a></li>
                    <li><a href="#">Item</a></li>
                  </ul>
                </li>
                <li>
                  <a className="link-nav" href="#">커뮤니티</a>
                  <ul className="b-cate">
                    <li><a href="#">Telegram</a></li>
                    <li><a href="#">Twiter</a></li>
                    <li><a href="#">Youtube</a></li>
                    <li><a href="#">Medium</a></li>
                  </ul>
                </li>
                <li>
                  <a className="link-nav" href="#">자료실</a>
                  <ul className="b-cate">
                    <li><a href="#">이미지</a></li>
                    <li><a href="#">다운로드</a></li>
                    <li><a href="#">GitHub</a></li>
                  </ul>
                </li>
                <li>
                  <a className="link-nav" href="#">고객센터</a>
                  <ul className="b-cate">
                    <li><a href="#">FAQ</a></li>
                    <li><a href="#">1:1문의</a></li>
                    <li><a href="#">Partner</a></li>
                  </ul>
                </li>
              </ul>
              <div className="start">
                <a className="btn_start" href="#">
                  <strong className="txt_start">GAME START</strong>
                </a>
              </div>
            </div>
          </div>
      </div>
      );
    }
  }

  
export default Header;