import React from 'react';
import axios from "axios";
import configuration from "./configuration.js";
import state from "./state.js";
import Card from 'react-bootstrap/Card'
import ProgressBar from 'react-bootstrap/ProgressBar'
import Form from 'react-bootstrap/Form'
import Button from 'react-bootstrap/Button'
import Dropdown from 'react-bootstrap/Dropdown'
import DropdownButton from 'react-bootstrap/DropdownButton'
import ButtonToolbar from 'react-bootstrap/ButtonToolbar'

export default class ShuttleBooks extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            customerName: '',
            customerNameValidation: '',
            customerEMail: '',
            customerEMailValidation: '',
            books: []
        };

        this.handleCustomerNameChange = this.handleCustomerNameChange.bind(this);
        this.handleCustomerEMailChange = this.handleCustomerEMailChange.bind(this);
        this.orderCustom = this.orderCustom.bind(this);
        this.orderCustomEventSource = this.orderCustomEventSource.bind(this);
        this.orderEventSourceModule = this.orderEventSourceModule.bind(this);
    }

    componentDidMount() {
        axios.get(configuration.url + "/products").then(response => {
            this.setState({
                books: response.data.data.map(function (book) {
                    book.buying = false;
                    return book;
                })
            });
        });
    }

    toggle(book) {
        this.setState(() => {
            return {
                books: this.state.books.map(function (map) {
                    if (map.id === book.id) {
                        map.buying = !map.buying;
                    }

                    return map;
                })
            }
        });
    }

    total() {
        return this.state.books.reduce(function (result, book) {
            return result + (book.buying ? book.price : 0);
        }, 0);
    }

    validate() {
        const email = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        let customerNameValidation = '';
        let customerEMailValidation = '';

        if (!this.state.customerName) {
            customerNameValidation = "can't be blank";
        }

        if (!this.state.customerEMail) {
            customerEMailValidation = "can't be blank";
        }

        if (!customerEMailValidation && !email.test(this.state.customerEMail)) {
            customerEMailValidation = 'not a valid e-mail address';
        }

        this.setState({
            customerNameValidation: customerNameValidation,
            customerEMailValidation: customerEMailValidation
        });

        return !customerNameValidation || !customerEMailValidation;
    }

    handleCustomerNameChange(e) {
        this.setState({
            customerName: e.target.value
        });

        this.validate();
    }

    handleCustomerEMailChange(e) {
        this.setState({
            customerEMail: e.target.value
        });

        this.validate();
    }

    cancel() {
        this._clearOrder();
    }

    _clearOrder() {
        this.setState(() => {
            return {
                books: this.state.books.map(function (map) {
                    map.buying = false;

                    return map;
                })
            }
        });
    }

    orderCustom() {
        this._submitOrder("custom");
    }

    orderCustomEventSource() {
        this._submitOrder("custom / event-source");
    }

    orderEventSourceModule() {
        this._submitOrder("event-source / module");
    }

    _submitOrder(targetSystem) {
        var self = this;

        if (!this.validate()) {
            return false;
        }

        var order = {
            productIds: [],
            targetSystem: targetSystem,
            customerName: this.state.customerName,
            customerEMail: this.state.customerEMail
        };

        this.state.books.forEach(function (book) {
            if (book.buying) {
                order.productIds.push(book.id);
            }
        });

        axios
            .post(configuration.url + "/orders", order)
            .then(() => {
                self._clearOrder();

                state.alerts.add({
                    message: "Your order has been sent for processing.",
                    name: "order-placed"
                });
            })
            .catch(error => {
                state.alerts.add({
                    message: error.message,
                    name: "order-error",
                    type: "danger"
                });
            });
    }

    render() {
        if (!!this.state.books.length) {
            return (
                <div>
                    <h4>Available titles</h4>
                    <table className="table table-sm">
                        <thead className="thead-light">
                            <tr className="row">
                                <th className="col-1"></th>
                                <th className="col">Title</th>
                                <th className="col-2 text-right">Price</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.state.books.map((book) => {
                                    return (
                                        <tr className={'row ' + (book.buying ? 'table-info' : '')} key={book.id}>
                                            <td className="col-1">
                                                {
                                                    book.buying
                                                        ? <button className="btn btn-default btn-danger btn-sm" onClick={() => this.toggle(book)}>Remove</button>
                                                        : <button className="btn btn-default btn-success btn-sm" onClick={() => this.toggle(book)}>Add</button>
                                                }
                                            </td>
                                            <td className="col"><a href={book.url} target={"_blank"}>{book.description}</a></td>
                                            <td className="col-2 text-right">{book.price}</td>
                                        </tr>
                                    )
                                })
                            }
                        </tbody >
                        <tfoot>
                            <tr className="row">
                                <td colSpan="2" className="col"></td>
                                <td className="col-2 text-right info">{this.total()}</td>
                            </tr>
                        </tfoot>
                    </table >
                    {this.total() > 0 &&
                        (
                            <div>
                                <h4>Checkout</h4>
                                <label htmlFor="customerName">Name</label>
                                <Form.Control value={this.state.customerName} placeholder="Enter your name" trim="true" onChange={this.handleCustomerNameChange} />
                                {!!this.state.customerNameValidation && <div className="text-warning">{this.state.customerNameValidation}</div>}
                                <label htmlFor="customerEMail" className="mt-2">e-mail</label>
                                <Form.Control value={this.state.customerEMail} placeholder="abc.xyz@example.com" type="email" className="mr-1" onChange={this.handleCustomerEMailChange} />
                                {!!this.state.customerEMailValidation && <div className="text-warning">{this.state.customerEMailValidation}</div>}
                                <br />
                                <ButtonToolbar>
                                    <Button onClick={() => this.cancel()} variant="secondary" className="mr-1">Cancel</Button>
                                    <DropdownButton id="dropdown-item-button" title="Order">
                                        <Dropdown.Item as="button" onClick={() => this.orderCustom()}>Custom</Dropdown.Item>
                                        <Dropdown.Item as="button" onClick={() => this.orderCustomEventSource()}>Custom / EventSource</Dropdown.Item>
                                        <Dropdown.Item as="button" onClick={() => this.orderEventSourceModule()}>EventSource / Module</Dropdown.Item>
                                    </DropdownButton>
                                </ButtonToolbar>
                            </div>
                        )
                    }
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
}