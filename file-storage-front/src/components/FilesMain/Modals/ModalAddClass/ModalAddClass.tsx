import React, {useEffect, useState} from 'react';
import "../Modal.scss"
import {Button} from "../../../utils/Button/Button";
import Modal from "../../../utils/Modal/Modal";
import {useDispatch} from "react-redux";
import {filesSlice} from "../../../../redux/filesSlice";
import {useAppSelector} from "../../../../utils/hooks/reduxHooks";
import {InputText} from "../../../utils/Inputs/Text/InputText";
import {fetchAllClassifications} from "../../../../redux/classesDocs/classesDocsThunks";
import Classifications from "./SelectListClasses";
import classes from "./ModalAddClass.module.scss";
import classNames from "classnames";
import {patchAssignClassification} from "../../../../redux/thunks/fileThunks";
import {ReactComponent as Search} from "../../../../assets/search.svg";

const {closeModal} = filesSlice.actions;

export const ModalAddClass: React.FC<{ id: string, callbackAccept?: ((...args: any) => void) | null }> = ({id}) => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchAllClassifications());
    }, [])
    const [query, changeQuery] = useState("");
    const allClassifications = useAppSelector((state) => state.classesDocs.classifications);
    const [classActiveId, setClassActive] = useState<string | undefined>();

    function onClickClass(classId: string) {
        setClassActive((prev) => prev === classId ? undefined : classId);
    }

    function onSubmitClassesSearch() {
        dispatch(fetchAllClassifications(query));
    }

    function onSubmit() {
        if (classActiveId)
            dispatch(patchAssignClassification({classId: classActiveId, documentId: id}));
    }

    return (
        <Modal>
            <div className={classNames("modal-confirm", classes.wrapper)}>
                <h2 className={"modal-confirm__h2"}>Присвоить классификацию</h2>
                <p className={classNames("modal-confirm__p", classes.searchWrapper)} style={{width: "100%"}}>
                    <InputText value={query} onChange={(e: any) => changeQuery(e.target.value)}
                               style={{width: "100%"}} type={"text"}/>
                    <button className={classes.searchBtn} type="submit" onClick={onSubmitClassesSearch}>
                        <Search/>
                    </button>
                </p>
                <section className={classes.classItems}>
                    <div className={classes.classItemsWrapper}>
                        {allClassifications &&
                        <Classifications classifications={allClassifications}
                                         classActiveId={classActiveId}
                                         onClick={onClickClass}/>}
                    </div>
                </section>
                <div className={"modal-confirm__btns"}>
                    <Button onClick={onSubmit} disabled={!classActiveId}>Присвоить</Button>
                    <Button onClick={() => dispatch(closeModal())} type={"transparent"}>Отмена</Button>
                </div>
            </div>
        </Modal>
    )
}

