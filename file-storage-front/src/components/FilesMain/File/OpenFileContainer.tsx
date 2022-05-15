import React, {memo, useEffect, useState} from 'react';
import "./File.scss"
import {useAppDispatch, useAppSelector} from "../../../utils/hooks/reduxHooks";
import {fetchDownloadLink, fetchFile, fetchFileText} from "../../../redux/thunks/fileThunks";
import {filesSlice} from "../../../redux/filesSlice";
import OpenedFile from "./OpenFile";

export interface match<Params extends { [K in keyof Params]?: string } = {}> {
    params: Params;
    isExact: boolean;
    path: string;
    url: string;
}

export const OpenedFileContainer: React.FC<{ match: match<{ id: string }> }> = memo(({match}) => {
    const dispatch = useAppDispatch();
    const id = match.params["id"];
    const rights = useAppSelector((state) => state.profile.rights);
    const file = useAppSelector((state) => state.filesReducer.openFile);
    const filesTypes = useAppSelector((state) => state.filesReducer.filesTypes);

    const [urlPreview, setUrlPreview] = useState<string | null>(null);

    useEffect(() => {
        // if (!file?.url)
        //     dispatch(fetchDownloadLink(id));
        if (file && id === file?.fileId) return;
        dispatch(fetchFile(id));
    }, [id])

    useEffect(() => {
        if (file && (+fileType === 4 || +fileType === 5))
            dispatch(fetchFileText({id, type: +fileType}))
        if (file?.url && !urlPreview) {
            getUrlPreview(file.url);
        }
    }, [file])

    useEffect(() => {
        if (!file?.fileName)
            return
        dispatch(fetchDownloadLink(id))
    }, [file?.fileName])

    async function getUrlPreview(urlFile: string) {
        try {
            const response = await fetch(urlFile);
            const binaryData = [];
            binaryData.push(await response.blob())
            const previewUrl = window.URL.createObjectURL(new Blob(binaryData,
                {type: response.headers.get("content-type") || "text"}
            ));

            setUrlPreview(previewUrl);
        } catch (e) {

        }
    }

    if (!file)
        return null;

    const {fileType} = file;
    return <OpenedFile file={file} filesTypes={filesTypes || {}} id={id} rights={rights || []}
                       urlPreview={urlPreview}/>;
})



