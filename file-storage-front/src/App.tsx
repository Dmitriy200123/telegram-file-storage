import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Redirect, Route, Switch} from "react-router-dom";
// import FilesMain from "./components/FilesMain/FilesMain";
import {Provider} from "react-redux";
import File from "./components/File/File";
import {setupStore} from "./redux/redux-store";
import FilesMain from "./components/FilesMain/FilesMain";

const App:FC = () => {
    return (<div className="App">
      <Switch>
        <Route path={"/files"} exact component={FilesMain} />
        <Route path={"/file"} component={File} />
        <Redirect to={"/files"}/>
      </Switch>
    </div>)
}

const store = setupStore();
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
