import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';
//import './style.css'
import Kr from './components/kr/kr';

const rootElement = document.getElementById('react');    
ReactDOM.render(<Kr/>, rootElement);

if (module.hot) {
  module.hot.accept();
}