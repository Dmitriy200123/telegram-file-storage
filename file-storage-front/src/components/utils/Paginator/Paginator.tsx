import React, {FC, memo} from 'react';
import "./Paginator.scss"

interface IProps {
    current: number,
    count: number,
    pageHandler: (page:number) => void
}

const Paginator:FC<IProps> = memo(({current, count, pageHandler}) => {
    let pages: Array<number> = [];
    const dif = 4;
    for (let i = Math.max(current - dif, 2); i <= Math.min(current + dif, count - 1); i++) {
        pages.push(i);
    }

    if (count <= 1)
        return null;
    return (
        <div className={"paginator"}>
            <button className={"paginator__item" + ((current === 1) ? " paginator__item_disabled" : "")}
                    disabled={current === 1}
                    onClick={() => pageHandler(current - 1)}>←
            </button>

            <div className={"paginator__item " + (1 === current ? "paginator__item_active" : "")}
                 onClick={() => pageHandler(1)}>1
            </div>
            {(current > 2 + dif ) && <div className={"paginator__nothing"}>...</div>}

            {pages.map((e) => <div className={"paginator__item " + (e === current ? "paginator__item_active" : "")}
                                   onClick={() => pageHandler(e)} key={e}>{e}</div>)}
            {count > 1 &&
            <>{(current < count - dif - 1) && <div className={"paginator__nothing"}>...</div>}
                <div onClick={() => pageHandler(count)}
                     className={"paginator__item " + (count === current ? "paginator__item_active" : "")}>{count}</div>
            </>}

            <button className={"paginator__item" + ((current === count) ? " paginator__item_disabled" : "") }
                    disabled={current === count}
                    onClick={() => pageHandler(current + 1)}>→
            </button>
        </div>
    )
})

export default Paginator;
