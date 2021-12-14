import React from 'react';
import "./Loadings.scss"
import LoadingGif from "./../../../assets/loading.gif";

const Loading:React.FC = () => {
    return (
        <div className={"loading"}>
            <div className={"loading__loading"}>
                <img src={LoadingGif}/>
            </div>
        </div>
    )
}

export default Loading;
