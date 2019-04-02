import React, { Component } from 'react';
import './assets/css/style.css';
import './assets/css/reset.css';
//import './assets/js/html5shiv';
import Header from './common/Header';
import Footer from "./common/Footer";
import Section1 from './section/Section1';
import Section3 from './section/Section2';
import Section2 from './section/Section3';


class Kr extends Component {
  render() {
    return (
      <div className="App">
        <Header />
        <div className="section">
          <Section1 />
          <Section2 />
          <Section3 />
        </div>
        <Footer />
      </div>
    );
  }
}
 
export default Kr;

