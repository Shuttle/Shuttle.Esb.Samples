import React from 'react';
import axios from "axios";
import configuration from "./configuration.js";
import state from "./state.js";
import Card from 'react-bootstrap/Card'
import ProgressBar from 'react-bootstrap/ProgressBar'
import Form from 'react-bootstrap/Form'
import FormControl from 'react-bootstrap/FormControl'
import Button from 'react-bootstrap/Button'

export default class ShuttleBooks extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            customerName: "",
            customerEMail: "",
            books: []
        };

        this.handleCustomerNameChange = this.handleCustomerNameChange.bind(this);
        this.handleCustomerEMailChange = this.handleCustomerEMailChange.bind(this);
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

    componentWillUnmount() {

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

    handleCustomerNameChange(e) {
        this.setState({ customerName: e.target.value });
        console.log(e.target.value);
    }

    handleCustomerEMailChange(e) {
        this.setState({ customerEMail: e.target.value });
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
                                <div className="text-warning" v-if="!$v.customerName.required">can't be blank</div>
                                <FormControl.Feedback type="invalid">Just checking</FormControl.Feedback>
                                <label htmlFor="customerEMail" className="mt-2">e-mail</label>
                                <Form.Control value={this.state.customerEMail} placeholder="abc.xyz@example.com" type="email" className="mr-1" onChange={this.handleCustomerEMailChange} />
                                <br />
                                <Button onClick={() => this.cancel()} variant="secondary" className="mr-1">Cancel</Button>
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