from sqlalchemy import Integer, String, Column, DateTime, ForeignKey
from sqlalchemy.orm import declarative_base, relationship


Base = declarative_base()


class Product(Base):
    __tablename__ = 'products'

    product_id = Column(Integer, primary_key=True)
    name = Column(String(255), nullable=False)
    category = Column(String(255), nullable=False)
    brand = Column(String(255), nullable=False)


class Store(Base):
    __tablename__ = 'stores'

    store_id = Column(Integer, primary_key=True)
    address = Column(String(255), nullable=False)
    region = Column(Integer, nullable=False)


class Customer(Base):
    __tablename__ = 'customers'

    customer_id = Column(Integer, primary_key=True)
    name = Column(String(255), nullable=False)
    surname = Column(String(255), nullable=True)
    birth_date = Column(DateTime, nullable=False)


class Price(Base):
    __tablename__ = 'prices'

    price_id = Column(Integer, primary_key=True)
    product_id = Column(Integer, ForeignKey("products.product_id"), nullable=False)
    product = relationship("Product")
    price = Column(Integer, nullable=False)
    start_date = Column(DateTime, primary_key=True, nullable=False)
    end_date = Column(DateTime, primary_key=True, nullable=True)


class Sale(Base):
    __tablename__ = 'sales'

    sale_id = Column(Integer, primary_key=True)
    product_id = Column(Integer, ForeignKey("products.product_id"), nullable=False)
    product = relationship("Product")
    store_id = Column(Integer, ForeignKey("stores.store_id"), nullable=False)
    store = relationship("Store")
    customer_id = Column(Integer, ForeignKey("customers.customer_id"), nullable=True)
    customer = relationship("Customer")
    sale_date = Column(DateTime, nullable=False)