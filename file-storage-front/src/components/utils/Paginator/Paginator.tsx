import React, {useCallback, useEffect} from 'react';
import "./Paginator.scss"
import {fetchFiles} from "../../../redux/mainThunks";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";
import {TypePaginator} from "../../../models/File";
import {filesSlice} from "../../../redux/filesSlice";

const {changePaginatorPage} = filesSlice.actions;

const Paginator = ({paginator, onChangePage}: { paginator: TypePaginator, onChangePage: () => void }) => {
    const {currentPage, filesInPage,count} = paginator
    const dispatch = useAppDispatch();

    let pages: Array<number> = [];
    const dif = 4;
    for (let i = Math.max(currentPage - dif, 2); i <= Math.min(currentPage + dif, count - 1); i++) {
        pages.push(i);
    }
    const changePage = useCallback((page: number) => dispatch(changePaginatorPage(page)),[]);

    useEffect(() => {
        onChangePage();
        // dispatch(fetchFiles({skip:(currentPage - 1) * filesInPage, take: filesInPage}));
    },[currentPage])

    if (count <= 1)
        return null;
    return (
        <div className={"paginator"}>
            <button className={"paginator__item" + ((currentPage === 1) ? " paginator__item_disabled" : "")} disabled={currentPage === 1}
                    onClick={() => changePage(currentPage - 1)}>←
            </button>

            <div className={"paginator__item " + (1 === currentPage ? "paginator__item_active" : "")}
                 onClick={() => changePage(1)}>1
            </div>
            {(currentPage > 2 + dif ) && <div className={"paginator__nothing"}>...</div>}

            {pages.map((e) => <div className={"paginator__item " + (e === currentPage ? "paginator__item_active" : "")}
                                   onClick={() => changePage(e)} key={e}>{e}</div>)}
            {count > 1 &&
            <>{(currentPage < count - dif - 1) && <div className={"paginator__nothing"}>...</div>}
                <div onClick={() => changePage(count)}
                     className={"paginator__item " + (count === currentPage ? "paginator__item_active" : "")}>{count}</div>
            </>}

            <button className={"paginator__item" + ((currentPage === count) ? " paginator__item_disabled" : "") } disabled={currentPage === count}
                    onClick={() => changePage(currentPage + 1)}>→
            </button>
        </div>
    )
}

export default Paginator;
