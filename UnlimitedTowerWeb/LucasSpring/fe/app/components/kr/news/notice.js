import React, { Component } from 'react';

class Notice extends Component {
    render() {
        return (
            <div id="notice">
                <div className="notice">
                    <div className="container">
                        <div className="noti">
                            <div className="noti_tit">
                                <h2>공지사항</h2>
                            </div>
                            
                            <div className="notice-right">
                                <table>
                                    <tbody>
                                        <tr>
                                            <th>구분</th>
                                            <th>제목</th>
                                            <th>작성자</th>
                                            <th>작성일</th>
                                        </tr>
                                        <tr>
                                            <td>공지사항</td>
                                            <td>Birth..</td>
                                            <td>fldkgjg</td>
                                            <td>1234444</td>
                                        </tr>
                                        <tr>
                                            <td>공지사항</td>
                                            <td>Birth..</td>
                                            <td>fldkgjg</td>
                                            <td>1234444</td>
                                        </tr>
                                        <tr>
                                            <td>공지사항</td>
                                            <td>Birth..</td>
                                            <td>fldkgjg</td>
                                            <td>1234444</td>
                                        </tr>
                                        <tr>
                                            <td>공지사항</td>
                                            <td>Birth..</td>
                                            <td>fldkgjg</td>
                                            <td>1234444</td>
                                        </tr>
                                        <tr>
                                            <td>공지사항</td>
                                            <td>Birth..</td> 
                                            <td>fldkgjg</td>
                                            <td>1234444</td>
                                        </tr>
                                        <tr>
                                            <td>공지사항</td>
                                            <td>Birth..</td> 
                                            <td>fldkgjg</td>
                                            <td>1234444</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <div className="pagination">
                                    <a href="#" className="prev">prev</a>
                                    <a href="#" className="num"><em>1</em></a>
                                    <a href="#" className="num"><em>2</em></a>
                                    <a href="#" className="next">next</a>
                                </div>
                                <div className="search">
                                    <select name="schFld" id="schFld">
                                        <option value selected="selected">제목 + 내용</option>
                                        <option value="ttl">제목</option>
                                        <option value="cont">내용</option>
                                    </select>
                                    <input type="text" />
                                    <img className="glass" src alt />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}
 
export default Notice;