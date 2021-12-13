import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Link, Redirect, Route, Switch} from "react-router-dom";
import {Provider} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFile} from "./components/File/OpenFile";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppSelector} from "./utils/hooks/reduxHooks";
import {LoadFileMain} from "./components/LoadFile/LoadFileMain";
import {modalContents} from "./components/utils/Modal/Modal";
import {StartPage} from "./components/StartPage/StartPage";
import {Messages} from "./components/utils/Messages/Messages";
import {Navbar} from "./components/Navbar/Navbar";

const App: FC = () => {
    const {filesReducer, profile} = useAppSelector((state) => state);
    const {messages} = profile;
    const {loading, modalConfirm} = filesReducer;
    const {isOpen, id, content} = modalConfirm;
    const Content = modalContents[content || 0];

    return (<div className="App app">
        {!!messages.length && <Messages messages={messages} className={"app__messages"}/>}

        <Switch>
            <Route path={"/login"} component={StartPage}/>
            <Route>
                <Navbar className={"app__navbar"}/>
                <div className={"app__content"}>
                    <header className="header"/>
                    {loading && <div>Загрузочка</div>}
                    <div style={{flex: "1 1 auto", display: "flex", flexDirection: "column"}}>
                        <Switch>
                            <Route path={"/files"} exact component={FilesMain}/>
                            <Route path={"/file/:id"} component={OpenedFile}/>
                            <Route path={"/load/"} component={LoadFileMain}/>
                            <Route path={"/login"} component={StartPage}/>
                            <Redirect to={"/files"}/>
                        </Switch>
                    </div>
                    {isOpen && id && <Content id={id}/>}
                </div>
            </Route>
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
