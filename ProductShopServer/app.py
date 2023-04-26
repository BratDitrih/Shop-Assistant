from flask import Flask, request
import database as db

app = Flask(__name__)
app.config['JSON_AS_ASCII'] = False

@app.route("/customers/show/")
def get_random_customers_api():
    customers = db.get_random_customers()
    return customers

@app.route("/stores/<store_id>/")
def get_store_api(store_id):
    store = db.get_store(store_id)
    return store

@app.route("/prices/max")
def get_product_with_max_price_api():
    max_price = db.get_product_with_max_price()
    return max_price

@app.route("/prices/stats/<product_id>")
def get_stats_of_product_api(product_id):
    stat = db.get_stats_of_product(product_id)
    return stat

@app.route("/stores/add", methods=["POST"])
def add_store_api():
    data = request.get_json()
    result = db.add_store(data)
    return result

@app.route("/stores/delete/<store_id>", methods=["DELETE"])
def delete_store_api(store_id):
    result = db.delete_store(store_id)
    return result

app.run("0.0.0.0", port=8080, debug=True)