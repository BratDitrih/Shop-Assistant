from sqlalchemy import create_engine, func
from sqlalchemy.orm import sessionmaker
from flask import jsonify
from models import Product, Store, Customer, Price, Sale

engine = create_engine("postgresql+psycopg2://postgres:400501@localhost:5432/ProductSales")


def get_all_customers():
    Session = sessionmaker(bind=engine)
    with Session() as session:
        customers = session.query(Customer).all()
        result = []
        for customer in customers:
            result.append({
            'customer_id': customer.customer_id,
            'name': customer.name,
            'surname': customer.surname,
            "birth_date": customer.birth_date
            })
    return jsonify(result)


def get_customer_purchases(customer_id):
        Session = sessionmaker(bind=engine)
        with Session() as session:
            latest_sale_date_subquery = session.query(func.max(Sale.sale_date).filter(Sale.customer_id == customer_id)).subquery()
            purchases = session.query(Sale.sale_id, Sale.sale_date, Product.name, Product.brand).join(Product).filter(Sale.customer_id == customer_id).filter(Sale.sale_date == latest_sale_date_subquery).all()
            result = []
            for purchase in purchases:
                result.append({
                'sale_id': purchase.sale_id,
                'sale_date': purchase.sale_date,
                'name': purchase.name,
                "brand": purchase.brand
                })
        return jsonify(result)
 

def get_store(store_id):
    Session = sessionmaker(bind=engine)
    result = { }
    with Session() as session:
        store = session.query(Store).filter(Store.store_id == store_id).first()
        if store:
            result = {
            "store_id": store.store_id,
            "address": store.address,
            "region": store.region
            }
    return jsonify(result)
 

def get_product_with_max_price():
    Session = sessionmaker(bind=engine)
    result = {}
    with Session() as session:
        max_price = session.query(Price).order_by(Price.price.desc(), Price.start_date.desc()).first()
        if max_price:
            item = session.query(Product).filter(Product.product_id == max_price.product_id).first()
            result = {
            "product_id": item.product_id,
            "name": item.name,
            "category": item.category,
            "brand": item.brand,
            "price": max_price.price,
            "start_date": max_price.start_date
            }
    return jsonify(result)
 

def get_stats_of_product(product_id):
    Session = sessionmaker(bind=engine)
    result = {}
    with Session() as session:
        item = session.query(Product).filter(Product.product_id == product_id).first()
        if item:
            count = session.query(func.count(Price.price)).filter(Price.product_id == product_id).scalar()
            stores_count = session.query(func.count(Sale.store_id)).filter(Sale.product_id == product_id).scalar()
            max_price = session.query(func.max(Price.price)).filter(Price.product_id == product_id).scalar()
            min_price = session.query(func.min(Price.price)).filter(Price.product_id == product_id).scalar()
            avg_price = session.query(func.avg(Price.price)).filter(Price.product_id == product_id).scalar()
            
            result = {
            "count": count,
            "stores_amount": stores_count,
            "min_price": min_price,
            "max-price": max_price,
            "avg_price": avg_price
            }
    return jsonify(result)
 

def add_store(data):
    Session = sessionmaker(bind=engine)
    with Session() as session:
        new_record = Store(**data)
        session.add(new_record)
        session.commit()
        result = new_record.store_id
    return jsonify({'store_id': result})
 

def delete_store(store_id):
    Session = sessionmaker(bind=engine)
    with Session() as session:
        store = session.query(Store).get(store_id)
        if not store:
            result = {"status": "not found"}
        else:
            session.delete(store)
            session.commit()
            result = {"status": "ok"}
    return jsonify(result)