import React, {FC, useState} from 'react';
import "./File.scss"
import Document from "./../../../assets/document.png";
import Modal from "../../utils/Modal/Modal";
import classNames from "classnames";

type PropsType = { message?: string | null, fileType: string, urlPreview?: null | string };

const PreviewContent: FC<PropsType> = ({message, fileType, urlPreview}) => {
    return <>
        {message && <>
            <h2 className={"file__previewTitle"}>Соодержимое:</h2>
            <div className="file__message">
                {+fileType === 4 ? <a href={message}>{message} </a> : message}
            </div>
        </>}
        {![5, 4, 3].includes(+fileType) && urlPreview && <>
            <h2 className={"file__previewTitle"}>Соодержимое:</h2>
            <div><Embed urlPreview={urlPreview} type={+fileType}/></div>
        </>
        }
    </>
}

const Embed: FC<{ type: number, urlPreview: string }> = ({type, urlPreview}) => {
    if (type === 3)
        return <img alt={"image"} className={"file__embed"} src={urlPreview} width="100%"/>
    if (type === 2 || type === 1)
        return <embed src={urlPreview}/>

    return <embed src={urlPreview} className={"file__embed"}/>
}


export default PreviewContent;




