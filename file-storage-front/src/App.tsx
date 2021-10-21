import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Redirect, Route, Switch} from "react-router-dom";
import FilesMain from "./components/FilesMain/FilesMain";
import {Provider} from "react-redux";
import store from "./redux/redux-store";
import File from "./components/File/File";

const App:FC = () => {
    return (<div className="App">
      <Switch>
        <Route path={"/files"} exact component={FilesMain} />
        <Route path={"/file"} component={File} />
        <Redirect to={"/files"}/>
      </Switch>
    </div>)
}

function FileStorageApp() {
    return (
        <BrowserRouter>
          <Provider store={store}>
            <App/>
          </Provider>
        </BrowserRouter>
    );
}

export default FileStorageApp;
