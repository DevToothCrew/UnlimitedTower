import React, { Component } from 'react';
import gbox from './../assets/img/gbox.png';

class Section3 extends Component {
    render() {
      return (
        <div id="section3">
            <div className="container">
              <div className="section3">
                  <div className="sec3">
                  <h1 className="youtube-tit" />
                    <div className="youtube-wrap">
                      <div className="ybox"><img src={gbox} alt /></div>
                      <div className="ybox"><img src={gbox} alt /></div>
                      <div className="ybox"><img src={gbox} alt /></div>
                    </div>
                  </div>
              </div>
            </div>
      </div>
        );
    }
  }

  
export default Section3;