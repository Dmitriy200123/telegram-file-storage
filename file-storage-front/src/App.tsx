import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Redirect, Route, Switch} from "react-router-dom";
import {Provider} from "react-redux";
import File from "./components/File/File";
import {setupStore} from "./redux/redux-store";
import FilesMain from "./components/FilesMain/FilesMain";

const App: FC = () => {
    return (<div className="App">
        <Switch>
            <Route path={"/files"} exact component={FilesMain}/>
            <Route path={"/file"} component={File}/>
            <Redirect to={"/files"}/>
        </Switch>
    </div>)
}

// const TestApp:FC = () => {
//     const { isLoading, isError, error, } = Api.useFetchAllPostsQuery("");
//     const dispatch = useAppDispatch();
//     useEffect(() =>{
//         dispatch(fetchFiles());
//     },[])
//     const some = useAppSelector((state) => state.filesReducer.some);
//     return <div className="App">
//         {/*{isLoading ? "Загрузка" : ""}*/}
//         {/*{isError ? JSON.stringify(error) : ""}*/}
//         {JSON.stringify(some)}
//         {/*{!isError &&data?.map((elem:any) => <>{elem.id} {elem.title}</>)}*/}
//     </div>
// }

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
