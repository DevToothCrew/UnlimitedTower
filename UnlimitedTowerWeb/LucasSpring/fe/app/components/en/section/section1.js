import React, { Component } from 'react';
import Slider from "react-slick";



class Section1 extends Component {
    render() {
        const slideFor = {
            slidesToShow: 1,
            slidesToScroll: 1,
            dots:true,
            arrows: false,
          };
      return (
        <div id="section1">
            <div className="section1">
            
            <div className="slider slider-for">
            <Slider {...slideFor}>
                <div className="banner-bg">
                <h3>Event1</h3>
                </div>
                <div className="banner-bg">
                <h3>Event2</h3>
                </div>
                <div className="banner-bg">
                <h3>Event3</h3>
                </div>
                <div className="banner-bg">
                <h3>Event4</h3>
                </div>
                <div className="banner-bg">
                <h3>Event5</h3>
                </div>
            </Slider>
            </div>
            </div>
        </div>
        );
    }
  }
export default Section1;