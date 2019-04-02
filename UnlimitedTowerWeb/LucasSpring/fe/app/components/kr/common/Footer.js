import React, { Component } from 'react';
import footerLogo from './../assets/img/flogo.png';
import twitter from './../assets/img/twitter.png';
import teleEn from './../assets/img/tele(en).png';
import teleKr from './../assets/img/tele(kr).png';
import telegram from './../assets/img/telegram.png';
import md from './../assets/img/md.png';
import github from './../assets/img/github.png';


class Footer extends Component {
    render() {
      return (
        <footer id="footer">
            <div className="container">
            <div className="footer">
                <a href="javascript:goMenu('1')"><img className="footer-logo" src={footerLogo} alt="" /></a>				
                <ul className="f-sns">
                <li><a href="https://twitter.com/UnlimitedTower" target="_blank"><img src={twitter} alt="" /></a></li>
                <li><a href="https://t.me/UnlimitedTower_EN" target="_blank"><img src={teleEn} alt="" /></a></li>
                <li><a href="https://t.me/UnlimitedTower_kr" target="_blank"><img src={teleKr} alt="" /></a></li>
                <li><a href="https://t.me/UnlimitedTower" target="_blank"><img src={telegram} alt="" /></a></li>
                <li><a href="https://medium.com/unlimitedtower" target="_blank"><img src={md} alt="" /></a></li>
                <li><a href="https://github.com/DevToothCrew" target="_blank"><img src={github} alt="" /></a></li>            
                {/* li><a href="mailto:official@unlimitedtower.com"><img src="assets/img/mail.png" alt=""></a></li>  */}	                
                </ul>
                <span>Contact : official@unlimitedtower.com <br /></span>
                <span>Copyright © 2019 Devtooth. All Rights Reserved.</span>
            </div>
            </div>
        </footer>
        );
    }
  }

  
export default Footer;