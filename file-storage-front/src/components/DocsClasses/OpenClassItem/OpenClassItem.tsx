import React, {FC, memo} from 'react';
import {Link} from "react-router-dom";
import {ReactComponent as Edit} from "../../../assets/edit.svg";
import classes from "./OpenClassItem.module.scss";
import {Button} from "../../utils/Button/Button";
import {ReactComponent as Delete} from "../../../assets/delete.svg";
import {classesDocsSlice} from "../../../redux/classesDocs/classesDocsSlice";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";
import Tag, {CreateTag} from "./Tag/Tag";

const {openModal} = classesDocsSlice.actions

interface match<Params extends { [K in keyof Params]?: string } = {}> {
    params: Params;
    isExact: boolean;
    path: string;
    url: string;
}

type PropsType = { match: match<{ id: string }> }

const OpenClassItemContainer: React.FC<PropsType> = memo(({match}) => {
    const id = match.params["id"];
    const dispatch = useAppDispatch();

    function openRename() {
        dispatch(openModal({type: "edit"}));
    }

    return (
        <OpenClassItem openRename={openRename}/>
    );
});

type PropsTypeOpen = { openRename: () => void }


const OpenClassItem: FC<PropsTypeOpen> = memo(({openRename}) => {
    return <div className={classes.classItem}>
        <div className={classes.classItem__header}>
            <h2 className={classes.classItem__title}>Техническое задание</h2>
            <Link className={classes.classItem__close} to={"/docs-сlasses"}/>
        </div>
        <div className={classes.classItem__content}>
            <h3 onClick={openRename} className={classes.classItem__contentTitle}>{"Категория"} {<Edit/>}</h3>
            <div className={classes.classItem__tags}>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <Tag/>
                <CreateTag/>
            </div>

            <div className={classes.classItem__btns}>
                <Button
                    type={"danger"} className={classes.classItem__btn_delete}><span>Удалить</span><Delete/></Button>
            </div>
        </div>
    </div>;
});

export default OpenClassItemContainer;





