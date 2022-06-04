import React, {FC} from 'react';
import "./File.scss"

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
            <div className={"file__previewWrapper"}><Embed urlPreview={urlPreview} type={+fileType}/></div>
        </>
        }
    </>
}

const Embed: FC<{ type: number, urlPreview: string }> = ({type, urlPreview}) => {
    if (type === 3)
        return <img alt={"image"} className={"file__embed"} src={urlPreview} width="100%"/>
    if (type === 2 || type === 1)
        return <video controls={true} src={urlPreview} autoPlay={false}/>

    return <embed src={urlPreview} className={"file__embed"}/>
}


export default PreviewContent;




