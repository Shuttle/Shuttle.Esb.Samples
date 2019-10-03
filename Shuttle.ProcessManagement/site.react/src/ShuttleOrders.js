import React from 'react';
import axios from "axios";
import configuration from "./configuration.js";
import state from "./state.js";
import Card from 'react-bootstrap/Card'
import ProgressBar from 'react-bootstrap/ProgressBar'

export default class ShuttleOrders extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            orders: [],
            poll: false,
            pollTimer: 0
        };

        this.handleCancelOrder = this.handleCancelOrder.bind(this);
        this.handleArchiveOrder = this.handleArchiveOrder.bind(this);
        this._poll = this._poll.bind(this);
    }

    componentDidMount() {
        axios
            .get(configuration.url + "/orders")
            .then(response => {
                this.setState({
                    orders: response.data.data
                })
            })
            .then(() => {
                this.setState({ poll: true });
                this._poll();
            });
    }

    componentWillUnmount() {
        this.setState({ poll: false });
    }

    handleCancelOrder(order) {
        axios
            .delete(configuration.url + "/orders/" + order.id)
            .then(() => {
                state.alerts.add({
                    message: "Your cancellation request has been sent for processing.",
                    name: "order-cancel"
                });
            })
            .catch(() => {
                state.alerts.add({
                    message: "The selected order could not be cancelled.",
                    name: "order-cancel-error",
                    type: "danger"
                });
            });
    }

    handleArchiveOrder(order) {
        axios
            .delete(configuration.url + "/archivedorders/" + order.id)
            .then(() => {
                state.alerts.add({
                    message: "Your archive request has been sent for processing.",
                    name: "order-archive"
                });
            });
    }

    render() {
        if (!!this.state.orders.length) {
            return (
                <div>
                    <br/>
                    <h4>Active orders</h4>
                    <table className="table table-sm">
                        <thead className="thead-light">
                            <tr className="row">
                                <th className="col-1"></th>
                                <th className="col-2">Customer name</th>
                                <th className="col-2">Order number</th>
                                <th className="col-2">Order date</th>
                                <th className="col-3">Status</th>
                                <th className="col-2 text-right">Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.state.orders.map((order) => {
                                    return (
                                        <tr key={order.orderNumber} className="row">
                                            <td className="col-1">
                                                {order.canCancel &&
                                                    <button className="btn btn-default btn-danger btn-sm" onClick={() => this.handleCancelOrder(order)}>Remove</button>
                                                }
                                                {order.canArchive &&
                                                    <button className="btn btn-default btn-danger btn-sm" onClick={() => this.handleArchiveOrder(order)}>Archive</button>
                                                }
                                            </td>
                                            <td className="col-2">{order.customerName}</td>
                                            <td className="col-2">{order.orderNumber}</td>
                                            <td className="col-2">{order.orderDate}</td>
                                            <td className="col-3">{order.status}</td>
                                            <td className="col-2 text-right">{order.orderTotal}</td>
                                        </tr>
                                    )
                                })
                            }
                        </tbody >
                    </table >
                </div >
            );
        } else {
            return (
                <Card>
                    <Card.Header>(fetching books)</Card.Header>
                    <Card.Body>
                        <ProgressBar now={100} striped animated className="mt-2" />
                    </Card.Body>
                </Card>
            )
        }
    }

    _poll() {
        var timeout = this.state.pollTimer;

        if (timeout) {
            clearTimeout(timeout);
        }

        if (!this.state.poll) {
            return;
        }

        timeout = setTimeout(() => {
            axios
                .get(configuration.url + "/orders")
                .then(response => {
                    this.setState({ orders: response.data.data });
                })
                .catch(() => {
                    state.alerts.add({
                        message: "Could not fetch the orders.",
                        name: "order-fetch-error",
                        type: "danger"
                    });
                })
                .then(() => {
                    this._poll();
                });
        }, 500);

        this.setState({ pollTimer: timeout });
    }
}
